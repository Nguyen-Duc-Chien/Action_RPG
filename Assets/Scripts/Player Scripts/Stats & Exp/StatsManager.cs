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
