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

    [Header("Melee Knockback Stats (Combat)")]
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    [Header("Ranged Knockback Stats (Bow)")]
    public float arrowKnockbackForce = 3f; 
    public float arrowKnockbackTime = 0.15f;
    public float arrowStunTime = 0.1f;

    [Header("Movement Stats")]
    public float speed;

    [Header("Health Stats")]
    public float maxHealth;
    public float currentHealth;
    public float damageResistance; // Percentage of damage reduction

    [HideInInspector] public bool hasLowHealthResistanceSkill = false;
    [HideInInspector] public bool isLowHealthResistanceActive = false;

    [Header("Ultimate - Health Regeneration")]
    [HideInInspector] public bool hasHealthRegenSkill = false;
    public float regenInterval = 5f;
    public float regenAmount = 2f;

    public bool isBowUnlocked = false;

    [Header("Arrow Effects Stats (Column 5)")]
    [HideInInspector] public float arrowSlowChance = 0f;
    [HideInInspector] public float arrowSlowDuration = 2f;
    [HideInInspector] public float arrowSlowAmount = 0.2f;

    [HideInInspector] public float arrowFreezeChance = 0f;
    [HideInInspector] public float arrowFreezeDuration = 1f;

    [Header("Arrow Fire Stats (Column 6)")]
    [HideInInspector] public float arrowBurnChance = 0f;
    [HideInInspector] public float arrowBurnDuration = 2f;
    [HideInInspector] public float arrowBurnDamage = 1f;

    private float defaultMeleeDamage;
    private float defaultRangeDamage;
    private float defaultWeaponRange;
    private float defaultKnockbackForce;
    private float defaultKnockbackTime;
    private float defaultStunTime;
    private float defaultArrowKnockbackForce;
    private float defaultArrowKnockbackTime;
    private float defaultArrowStunTime;
    private float defaultSpeed;
    private float defaultMaxHealth;
    private float defaultDamageResistance;
    private float defaultRegenInterval;
    private float defaultRegenAmount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SaveDefaultStats();
        }
        else
            Destroy(gameObject);
    }

    private void SaveDefaultStats()
    {
        defaultMeleeDamage = meleeDamage;
        defaultRangeDamage = rangeDamage;
        defaultWeaponRange = weaponRange;
        defaultKnockbackForce = knockbackForce;
        defaultKnockbackTime = knockbackTime;
        defaultStunTime = stunTime;
        defaultArrowKnockbackForce = arrowKnockbackForce;
        defaultArrowKnockbackTime = arrowKnockbackTime;
        defaultArrowStunTime = arrowStunTime;
        defaultSpeed = speed;
        defaultMaxHealth = maxHealth;
        defaultDamageResistance = damageResistance;
        defaultRegenInterval = regenInterval;
        defaultRegenAmount = regenAmount;
    }

    public void ResetStats()
    {
        meleeDamage = defaultMeleeDamage;
        rangeDamage = defaultRangeDamage;
        weaponRange = defaultWeaponRange;
        knockbackForce = defaultKnockbackForce;
        knockbackTime = defaultKnockbackTime;
        stunTime = defaultStunTime;
        arrowKnockbackForce = defaultArrowKnockbackForce;
        arrowKnockbackTime = defaultArrowKnockbackTime;
        arrowStunTime = defaultArrowStunTime;
        speed = defaultSpeed;
        maxHealth = defaultMaxHealth;
        damageResistance = defaultDamageResistance;
        regenInterval = defaultRegenInterval;
        regenAmount = defaultRegenAmount;

        currentHealth = maxHealth;

        hasLowHealthResistanceSkill = false;
        isLowHealthResistanceActive = false;
        hasHealthRegenSkill = false;
        isBowUnlocked = false;

        arrowSlowChance = 0f;
        arrowSlowDuration = 2f;
        arrowSlowAmount = 0.2f;

        arrowFreezeChance = 0f;
        arrowFreezeDuration = 1f;

        arrowBurnChance = 0f;
        arrowBurnDuration = 2f;
        arrowBurnDamage = 1f;

        if (healthText != null) healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
        if (PlayerHealthUI.Instance != null) PlayerHealthUI.Instance.UpdateHealthUI(currentHealth, maxHealth);
        if (statsUI != null) statsUI.UpdateAllStats();
    }

    public void UpdateMaxHealth(float amount)
    {
        maxHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;

        // Cập nhật slider
        if (PlayerHealthUI.Instance != null)
            PlayerHealthUI.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;

        // Cập nhật slider
        if (PlayerHealthUI.Instance != null)
            PlayerHealthUI.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void UpdateDamageResistance(float amount)
    {
        damageResistance += amount;
        statsUI.UpdateAllStats();
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
