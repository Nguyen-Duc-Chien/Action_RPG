using UnityEngine;
using System.Collections.Generic;

public class Enemy_Health : MonoBehaviour
{
    [System.Serializable]
    public struct QuantityTier
    {
        public int amount;            
        [Range(0f, 100f)]
        public float rollChance;
    }

    [System.Serializable]
    public struct LootItem
    {
        public ItemSO itemSO;         
        [Range(0f, 100f)]
        public float dropChance;

        [Header("Quantity (base for common rates increase)")]
        public List<QuantityTier> quantityTiers;
    }

    public int expReward = 3;

    [Header("Loot Settings")]
    public GameObject genericLootPrefab; 
    public List<LootItem> lootTable;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    public float currentHealth;
    public float maxHealth;

    private Animator anim;
    private Collider2D enemyCollider;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
    }

    public void ChangeHealth(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
        //Debug.Log("Enemy health changed by: " + amount + ", current health: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;

        if (OnMonsterDefeated != null) OnMonsterDefeated(expReward);

        CalculateLoot();

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = 1f;
            anim.SetTrigger("dieTrig");
        }
        // Disable collider to prevent further interactions
        if (enemyCollider != null) enemyCollider.enabled = false;

        // Disable movement scripts
        Torch_Movement torchMovement = GetComponent<Torch_Movement>();
        if (torchMovement != null)
        {
            torchMovement.enabled = false;
        }

        Archer_Movement archerMovement = GetComponent<Archer_Movement>();
        if (archerMovement != null)
        {
            archerMovement.enabled = false;
        }

        Barrel_Suicide barrelSuicide = GetComponent<Barrel_Suicide>();
        if (barrelSuicide != null)
        {
            barrelSuicide.enabled = false;
        }

        Frostbite_Archer_Movement frostbiteArcherMovement = GetComponent<Frostbite_Archer_Movement>();
        if (frostbiteArcherMovement != null)
        {
            frostbiteArcherMovement.enabled = false;
        }

        Pawn_Red_Movement pawnRedMovement = GetComponent<Pawn_Red_Movement>();
        if (pawnRedMovement != null)
        {
            pawnRedMovement.enabled = false;
        }

        // Diable all debuffs
        Enemy_DebuffManager debuffManager = GetComponent<Enemy_DebuffManager>();
        if (debuffManager != null)
        {
            debuffManager.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(gameObject, 1f);
    }

    private void CalculateLoot()
    {
        if (genericLootPrefab == null)
        {
            Debug.LogWarning("There isn't Generic Loot Prefab into script Enemy_Health: " + gameObject.name);
            return;
        }

        if (lootTable == null || lootTable.Count == 0) return;

        foreach (LootItem loot in lootTable)
        {
            if (loot.itemSO == null) continue;

            float spawnRoll = Random.Range(0f, 100f);

            if (spawnRoll <= loot.dropChance)
            {
                int finalAmount = 1;

                if (loot.quantityTiers != null && loot.quantityTiers.Count > 0)
                {
                    float qtyRoll = Random.Range(0f, 100f);
                    float cumulativeChance = 0f;

                    foreach (QuantityTier tier in loot.quantityTiers)
                    {
                        cumulativeChance += tier.rollChance; 
                        if (qtyRoll <= cumulativeChance)
                        {
                            finalAmount = tier.amount;
                            break;
                        }
                    }
                }

                GameObject spawnedItem = Instantiate(genericLootPrefab, transform.position, Quaternion.identity);

                Loot lootScript = spawnedItem.GetComponent<Loot>();
                if (lootScript != null)
                {
                    lootScript.Initialize(loot.itemSO, finalAmount, true);
                    //Debug.Log($"<color=green>[LOOT SYSTEM]</color> <b>{gameObject.name}</b> dropped: <b>{finalAmount}x {loot.itemSO.itemName}</b> (Spawn Roll: {spawnRoll:F1}/{loot.dropChance}%)");
                }
            }
        }
    }
}