using System;
using System.Collections;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }

    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    [Header("Passive Regen")]
    public float passiveRegenRate = 6f;    // Energy recovered per second
    public float regenDelay = 1f;        // Delay before passive regen starts after using energy

    [Header("Kill Regen")]
    public float killRegenAmount = 10f;    // Energy gained per enemy kill

    /// <summary>
    /// Fired whenever energy changes. Parameters: (currentEnergy, maxEnergy)
    /// </summary>
    public event Action<float, float> OnEnergyChanged;

    private float regenPauseTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        currentEnergy = maxEnergy;
        NotifyUI();
    }

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += OnEnemyKilled;
    }

    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= OnEnemyKilled;
    }

    private void Update()
    {
        // Countdown regen pause timer
        if (regenPauseTimer > 0f)
        {
            regenPauseTimer -= Time.deltaTime;
            return;
        }

        // Passive regen
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += passiveRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            NotifyUI();
        }
    }

    /// <summary>
    /// Attempt to use energy. Returns true if there was enough energy.
    /// </summary>
    public bool UseEnergy(float amount)
    {
        if (currentEnergy < amount)
            return false;

        currentEnergy -= amount;
        currentEnergy = Mathf.Max(currentEnergy, 0f);

        // Pause passive regen after spending energy
        regenPauseTimer = regenDelay;

        NotifyUI();
        return true;
    }

    /// <summary>
    /// Gain energy (e.g. from killing enemies).
    /// </summary>
    public void GainEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        NotifyUI();
    }

    /// <summary>
    /// Reset energy to max (called on player death / restart).
    /// </summary>
    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        regenPauseTimer = 0f;
        NotifyUI();
    }

    /// <summary>
    /// Increase max energy.
    /// </summary>
    public void UpdateMaxEnergy(float amount)
    {
        maxEnergy += amount;
        currentEnergy += amount;
        NotifyUI();
    }

    private void OnEnemyKilled(int exp)
    {
        GainEnergy(killRegenAmount);
    }

    private void NotifyUI()
    {
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        if (EnergyUI.Instance != null)
        {
            EnergyUI.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
        }
    }
}
