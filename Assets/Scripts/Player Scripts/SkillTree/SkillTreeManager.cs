using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public SkillSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availablePoints;

    private bool isLoading = false; // Guard flag — tránh trừ point khi đang load

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpend += HandleAbilityPointsSpend;
        SkillSlot.OnSkillMaxed += HandleSkillMaxed;
        ExpManager.OnLevelUp += UpdateAbilityPoints;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpend -= HandleAbilityPointsSpend;
        SkillSlot.OnSkillMaxed -= HandleSkillMaxed;
        ExpManager.OnLevelUp -= UpdateAbilityPoints;
    }

    private void Start()
    {
        LoadSkillsFromSave();
        availablePoints = 48;
        foreach (SkillSlot slot in skillSlots)
        {
            slot.skillButton.onClick.AddListener(() => CheckAvalablePoints(slot));
        }
        UpdatePointsUI();
    }

    private void CheckAvalablePoints(SkillSlot slot)
    {
        Debug.Log($"[SkillTreeManager] Button clicked for {slot.skillSO.skillName}. Available points: {availablePoints}");
        if (availablePoints > 0)
        {
            slot.TryUpgradeSkill();
        }
        else
        {
            Debug.LogWarning("[SkillTreeManager] Not enough points to upgrade!");
        }
    }

    private void HandleAbilityPointsSpend(SkillSlot skillSlot)
    {
        // Khi đang load, chỉ cần SkillManager xử lý effect, không trừ point
        if (isLoading) return;

        if(availablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }

        foreach (SkillSlot slot in skillSlots)
        {
            slot.UpdateUI();
        }

        // Auto-save skills after spending a point
        SaveSkillsToPlayerPrefs();
    }

    private void HandleSkillMaxed(SkillSlot skillSlot)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
            {
                slot.Unlock();
            }
        }
    }

    public void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        UpdatePointsUI();
    }

    private void UpdatePointsUI()
    {
        if (pointsText != null)
            pointsText.text = "Points: " + availablePoints.ToString();
    }

    #region Skills Save/Load

    public void SaveSkillsToPlayerPrefs()
    {
        if (RunManager.Instance == null) return;
        RunManager.Instance.SaveSkills(skillSlots, availablePoints);
    }

    private void LoadSkillsFromSave()
    {
        if (RunManager.Instance == null) return;
        if (!RunManager.Instance.HasSkillsSave())
        {
            // First run / sau khi chết — unlock row đầu (không có prerequisite), lock row còn lại
            foreach (SkillSlot slot in skillSlots)
            {
                slot.currentLevel = 0;
                slot.isUnlocked = (slot.prerequisiteSkillSlots == null || slot.prerequisiteSkillSlots.Count == 0);
                slot.UpdateUI();
            }
            UpdateAbilityPoints(0);
            return;
        }

        RunManager.Instance.LoadSkills(skillSlots, out int savedPoints);
        availablePoints = savedPoints;

        // Re-apply tất cả skill effects đã mua
        // Gọi lại SkillManager.HandleAbilityPointSpend cho mỗi level đã nâng
        isLoading = true;
        foreach (SkillSlot slot in skillSlots)
        {
            for (int i = 0; i < slot.currentLevel; i++)
            {
                // Fire sự kiện để SkillManager áp dụng lại effect
                SkillSlot.InvokeAbilityPointSpendForLoad(slot);
            }
        }
        isLoading = false;

        // Re-check unlock các skill có prerequisite đã max
        foreach (SkillSlot slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
            {
                slot.Unlock();
            }
        }
    }

    public void ResetSkills()
    {
        foreach (SkillSlot slot in skillSlots)
        {
            slot.currentLevel = 0;
            // Giữ lại unlocked cho row đầu (không có prerequisite) 
            slot.isUnlocked = (slot.prerequisiteSkillSlots == null || slot.prerequisiteSkillSlots.Count == 0);
            slot.UpdateUI();
        }
        availablePoints = 0;
        UpdatePointsUI();

        // Xoá save data
        SaveSkillsToPlayerPrefs();
        Debug.Log("<color=yellow>[SkillTreeManager]</color> All skills have been reset.");
    }

    #endregion
}
