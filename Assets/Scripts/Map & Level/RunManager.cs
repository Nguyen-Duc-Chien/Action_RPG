using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    [Header("Level Configs (20 levels — index 0 = Level 1)")]
    public List<LevelConfig> levelConfigs;

    [Header("Debug")]
    [Tooltip("Bật để mở khoá tất cả level khi test. Nhớ tắt trước khi build!")]
    public bool unlockAllLevels = false;

    public ItemDatabase itemDatabase;

    [Header("Scene Names")]
    public string lobbySceneName  = "Scene_Starting";
    public string forestSceneName = "Scene_Forest_RSG";
    public string dungeonSceneName = "Scene_Dungeon_RSG";

    [HideInInspector] public int currentLevelIndex = 0;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.GetInt("UnlockedLevel", 0) == 0)
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Public API

    public void PlayLevel(int index)
    {
        if (index < 0 || index >= levelConfigs.Count)
        {
            Debug.LogError($"[RunManager] Invalid level index: {index}");
            return;
        }

        currentLevelIndex = index;
        string sceneName = (index < 10) ? forestSceneName : dungeonSceneName;

        Debug.Log($"<color=cyan>[RunManager]</color> Loading Level {index + 1} → Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public LevelConfig GetCurrentConfig()
    {
        return GetConfig(currentLevelIndex);
    }

    public LevelConfig GetConfig(int index)
    {
        if (index >= 0 && index < levelConfigs.Count)
            return levelConfigs[index];
        return null;
    }

    public void CompleteCurrentLevel()
    {
        int levelNumber = currentLevelIndex + 1;

        PlayerPrefs.SetInt("CompletedLevel_" + levelNumber, 1);

        if (levelNumber < 20)
        {
            int unlockedSoFar = PlayerPrefs.GetInt("UnlockedLevel", 1);
            if (levelNumber >= unlockedSoFar)
            {
                PlayerPrefs.SetInt("UnlockedLevel", levelNumber + 1);
                Debug.Log($"<color=gold>[RunManager]</color> Level {levelNumber + 1} unlocked!");
            }
        }

        PlayerPrefs.Save();
        Debug.Log($"<color=green>[RunManager]</color> Level {levelNumber} completed & saved!");
    }

    public void GoToLobby()
    {
        Debug.Log("<color=cyan>[RunManager]</color> Returning to lobby...");
        SceneManager.LoadScene(lobbySceneName);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════════
    #region Progress Queries (1-based level numbers)

    public bool IsLevelUnlocked(int levelNumber)
    {
        if (unlockAllLevels) return true;
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        return levelNumber <= unlocked;
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        return PlayerPrefs.GetInt("CompletedLevel_" + levelNumber, 0) == 1;
    }

    public int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt("UnlockedLevel", 1);
    }

    #endregion

    #region Gold Save/Load

    public void SaveGold(int gold)
    {
        PlayerPrefs.SetInt("Player_Gold", gold);
        PlayerPrefs.Save();
    }

    public int LoadGold()
    {
        return PlayerPrefs.GetInt("Player_Gold", 0);
    }

    #endregion

    #region HP Save/Load

    public void SaveHP(float currentHealth, float maxHealth)
    {
        PlayerPrefs.SetFloat("Player_CurrentHP", currentHealth);
        PlayerPrefs.SetFloat("Player_MaxHP", maxHealth);
        PlayerPrefs.Save();
    }

    public void LoadHP(out float currentHealth, out float maxHealth)
    {
        currentHealth = PlayerPrefs.GetFloat("Player_CurrentHP", 0f);
        maxHealth     = PlayerPrefs.GetFloat("Player_MaxHP", 0f);
    }

    public bool HasHPSave()
    {
        return PlayerPrefs.HasKey("Player_MaxHP");
    }

    #endregion

    #region Exp Save/Load

    public void SaveExp(int level, int currentExp, int expToLevel, int totalExp)
    {
        PlayerPrefs.SetInt("Player_Level", level);
        PlayerPrefs.SetInt("Player_CurrentExp", currentExp);
        PlayerPrefs.SetInt("Player_ExpToLevel", expToLevel);
        PlayerPrefs.SetInt("Player_TotalExp", totalExp);
        PlayerPrefs.Save();
    }

    public void LoadExp(out int level, out int currentExp, out int expToLevel, out int totalExp)
    {
        level      = PlayerPrefs.GetInt("Player_Level", 0);
        currentExp = PlayerPrefs.GetInt("Player_CurrentExp", 0);
        expToLevel = PlayerPrefs.GetInt("Player_ExpToLevel", 0);
        totalExp   = PlayerPrefs.GetInt("Player_TotalExp", 0);
    }

    public bool HasExpSave()
    {
        return PlayerPrefs.HasKey("Player_Level");
    }

    #endregion

    #region Inventory Save/Load

    public void SaveInventory(InventorySlot[] slots)
    {
        PlayerPrefs.SetInt("Inv_SlotCount", slots.Length);

        for (int i = 0; i < slots.Length; i++)
        {
            string keyItem = "Inv_Slot_" + i + "_Item";
            string keyQty  = "Inv_Slot_" + i + "_Qty";

            if (slots[i].itemSO != null && slots[i].quantity > 0)
            {
                PlayerPrefs.SetString(keyItem, slots[i].itemSO.itemName);
                PlayerPrefs.SetInt(keyQty, slots[i].quantity);
            }
            else
            {
                PlayerPrefs.SetString(keyItem, "");
                PlayerPrefs.SetInt(keyQty, 0);
            }
        }

        PlayerPrefs.Save();
        Debug.Log($"<color=cyan>[RunManager]</color> Inventory saved ({slots.Length} slots)");
    }

    public void LoadInventory(InventorySlot[] slots)
    {
        if (!PlayerPrefs.HasKey("Inv_SlotCount")) return;
        if (itemDatabase == null)
        {
            Debug.LogError("[RunManager] ItemDatabase is null! Cannot load inventory.");
            return;
        }

        int savedCount = PlayerPrefs.GetInt("Inv_SlotCount", 0);
        int count = Mathf.Min(savedCount, slots.Length);

        for (int i = 0; i < count; i++)
        {
            string keyItem = "Inv_Slot_" + i + "_Item";
            string keyQty  = "Inv_Slot_" + i + "_Qty";

            string itemName = PlayerPrefs.GetString(keyItem, "");
            int qty = PlayerPrefs.GetInt(keyQty, 0);

            if (!string.IsNullOrEmpty(itemName) && qty > 0)
            {
                slots[i].itemSO   = itemDatabase.GetItemByName(itemName);
                slots[i].quantity  = qty;
            }
            else
            {
                slots[i].itemSO   = null;
                slots[i].quantity  = 0;
            }

            slots[i].UpdateUI();
        }

        Debug.Log($"<color=cyan>[RunManager]</color> Inventory loaded ({count} slots)");
    }

    #endregion

    #region Skills Save/Load

    public void SaveSkills(SkillSlot[] slots, int availablePoints)
    {
        PlayerPrefs.SetInt("Skill_SlotCount", slots.Length);
        PlayerPrefs.SetInt("Skill_AvailablePoints", availablePoints);

        for (int i = 0; i < slots.Length; i++)
        {
            string keyLevel    = "Skill_Slot_" + i + "_Level";
            string keyUnlocked = "Skill_Slot_" + i + "_Unlocked";

            PlayerPrefs.SetInt(keyLevel, slots[i].currentLevel);
            PlayerPrefs.SetInt(keyUnlocked, slots[i].isUnlocked ? 1 : 0);
        }

        PlayerPrefs.Save();
        Debug.Log($"<color=cyan>[RunManager]</color> Skills saved ({slots.Length} slots, {availablePoints} points)");
    }

    public void LoadSkills(SkillSlot[] slots, out int availablePoints)
    {
        availablePoints = PlayerPrefs.GetInt("Skill_AvailablePoints", 0);

        int savedCount = PlayerPrefs.GetInt("Skill_SlotCount", 0);
        int count = Mathf.Min(savedCount, slots.Length);

        for (int i = 0; i < count; i++)
        {
            string keyLevel    = "Skill_Slot_" + i + "_Level";
            string keyUnlocked = "Skill_Slot_" + i + "_Unlocked";

            slots[i].currentLevel = PlayerPrefs.GetInt(keyLevel, 0);
            slots[i].isUnlocked  = PlayerPrefs.GetInt(keyUnlocked, 0) == 1;
            slots[i].UpdateUI();
        }

        Debug.Log($"<color=cyan>[RunManager]</color> Skills loaded ({count} slots, {availablePoints} points)");
    }

    public bool HasSkillsSave()
    {
        return PlayerPrefs.HasKey("Skill_SlotCount");
    }

    #endregion

    #region Debug

    [ContextMenu("Reset All Progress (DEBUG)")]
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        Debug.LogWarning("[RunManager] All progress has been reset (levels, gold, exp, inventory)!");
    }

    [ContextMenu("Unlock All Levels (DEBUG)")]
    public void UnlockAllLevelsDebug()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 20);
        PlayerPrefs.Save();
        Debug.LogWarning("[RunManager] All 20 levels unlocked!");
    }

    #endregion
}
