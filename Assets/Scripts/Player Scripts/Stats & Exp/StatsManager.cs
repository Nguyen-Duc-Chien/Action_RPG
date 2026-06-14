using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public StatsUI statsUI;
    public TMP_Text healthText;

    [Header("Combat Stats")]
    public float meleeDamage;
    public float rangeDamage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    [Header("Movement Stats")]
    public float speed;

    [Header("Health Stats")]
    public float maxHealth;
    public float currentHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateMaxHealth(float amount)
    {
        maxHealth += amount;
        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
    }

    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
    }

    public void UpdateSpeed(float amount)
    {
        speed += amount;
        statsUI.UpdateAllStats();
    }

    public void UpdateMeleeDamage(float amount)
    {
        meleeDamage += amount;
        statsUI.UpdateAllStats();
    }

    public void UpdateRangeDamage(float amount)
    {
        rangeDamage += amount;
        statsUI.UpdateAllStats();
    }
}
