using System;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;
    public ExpManager expManager;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverExpText;

    public bool isInvincible = false;

    private Vector3 startPosition;
    private Coroutine regenCoroutine;

    public void Start()
    {
        startPosition = transform.position;

        LoadHPFromSave();
        OnHealthChanged?.Invoke(StatsManager.Instance.currentHealth, StatsManager.Instance.maxHealth);
        UpdateHealthUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenHealthRoutine());
        }
    }

    private System.Collections.IEnumerator RegenHealthRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(StatsManager.Instance.regenInterval);

            if (StatsManager.Instance.hasHealthRegenSkill)
            {
                float healthRatio = StatsManager.Instance.currentHealth / (float)StatsManager.Instance.maxHealth;

                if (healthRatio < 0.5f && StatsManager.Instance.currentHealth > 0)
                {
                    ChangeHealth(StatsManager.Instance.regenAmount);
                    Debug.Log($"Health_Regeneration activated. Amount: {StatsManager.Instance.regenAmount}");
                }
            }
        }
    }

    public void ChangeHealth(float amount)
    {
        // I-frames: skip damage while dashing
        if (amount < 0 && isInvincible) return;

        if (amount < 0)
        {
            float currentResistance = Mathf.Clamp(StatsManager.Instance.damageResistance, 0f, 0.9f);
            amount = amount * (1f - currentResistance);
        }

        float targetHealth = StatsManager.Instance.currentHealth + amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(targetHealth, 0f, StatsManager.Instance.maxHealth);

        UpdateHealthUI();

        if (StatsManager.Instance.hasLowHealthResistanceSkill)
        {
            float healthRatio = StatsManager.Instance.currentHealth / (float)StatsManager.Instance.maxHealth;

            if (healthRatio < 0.25f && !StatsManager.Instance.isLowHealthResistanceActive)
            {
                StatsManager.Instance.damageResistance += 0.5f; 
                StatsManager.Instance.isLowHealthResistanceActive = true; 

                if (StatsManager.Instance.statsUI != null) StatsManager.Instance.statsUI.UpdateAllStats();
            }
            else if (healthRatio >= 0.25f && StatsManager.Instance.isLowHealthResistanceActive)
            {
                StatsManager.Instance.damageResistance -= 0.5f;
                StatsManager.Instance.isLowHealthResistanceActive = false; 

                if (StatsManager.Instance.statsUI != null) StatsManager.Instance.statsUI.UpdateAllStats();
            }
        }

        if (StatsManager.Instance.currentHealth <= 0)
        {
            Die();
        }
        else
        {
            SaveHPToPlayerPrefs();
        }
    }

    public void UpdateHealthUI()
    {
        if (StatsManager.Instance != null)
        {
            // Invoke the event for any listeners (like PlayerHealthUI if using events)
            OnHealthChanged?.Invoke(StatsManager.Instance.currentHealth, StatsManager.Instance.maxHealth);

            // Directly call the Singleton (which you were already doing, just fixed the method name)
            if (PlayerHealthUI.Instance != null)
            {
                PlayerHealthUI.Instance.UpdateHealthUI(StatsManager.Instance.currentHealth, StatsManager.Instance.maxHealth);
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player has died, GameOverPanel set active!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverExpText != null)
            {
                gameOverExpText.text = "Score: " + expManager.totalExp;
            }
        }

        Time.timeScale = 0f;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        }
        UpdateHealthUI(); 

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        ExpManager foundExpManager = expManager != null ? expManager : FindAnyObjectByType<ExpManager>();
        if (foundExpManager != null)
        {
            foundExpManager.ResetExp();
        }

        InventoryManager invManager = FindAnyObjectByType<InventoryManager>();
        if (invManager != null)
        {
            invManager.ResetInventory();
        }

        // Reset skills
        SkillTreeManager skillTreeManager = FindAnyObjectByType<SkillTreeManager>(FindObjectsInactive.Include);
        if (skillTreeManager != null)
        {
            skillTreeManager.ResetSkills();
        }

        // Reset energy
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.ResetEnergy();
        }

        // Xóa sạch tiến trình level khi chết — chỉ giữ lại Level 1 mở khóa
        if (RunManager.Instance != null)
        {
            RunManager.Instance.ResetAllProgress();
        }

        // Xóa debuff
        Player_DebuffManager debuffMgr = GetComponent<Player_DebuffManager>();
        if (debuffMgr != null) debuffMgr.ResetAllDebuffs();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartRun();
            return;
        }

        transform.position = startPosition;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ResetPlayer(Vector3 spawnPosition)
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        UpdateHealthUI();

        Player_DebuffManager debuffMgr = GetComponent<Player_DebuffManager>();
        if (debuffMgr != null) debuffMgr.ResetAllDebuffs();

        transform.position = spawnPosition;
        startPosition = spawnPosition;

        ExpManager foundExpManager = expManager != null ? expManager : FindAnyObjectByType<ExpManager>();
        if (foundExpManager != null)
        {
            foundExpManager.ResetExp();
        }

        InventoryManager invManager = FindAnyObjectByType<InventoryManager>();
        if (invManager != null)
        {
            invManager.ResetInventory();
        }

        // Reset skills
        SkillTreeManager skillTreeMgr = FindAnyObjectByType<SkillTreeManager>(FindObjectsInactive.Include);
        if (skillTreeMgr != null)
        {
            skillTreeMgr.ResetSkills();
        }

        // Reset energy
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.ResetEnergy();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Debug.Log("Player stats reset and teleported to Spawn Point: " + spawnPosition);
    }

    #region HP Save/Load

    private void SaveHPToPlayerPrefs()
    {
        if (RunManager.Instance == null || StatsManager.Instance == null) return;
        RunManager.Instance.SaveHP(StatsManager.Instance.currentHealth, StatsManager.Instance.maxHealth);
    }

    private void LoadHPFromSave()
    {
        if (RunManager.Instance == null || StatsManager.Instance == null) return;
        if (!RunManager.Instance.HasHPSave()) return;

        RunManager.Instance.LoadHP(out float savedCurrent, out float savedMax);
        if (savedMax > 0)
        {
            StatsManager.Instance.maxHealth = savedMax;
            StatsManager.Instance.currentHealth = savedCurrent;
        }
    }

    #endregion
}