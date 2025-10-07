using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public SkillSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availablePoints;

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
        foreach (SkillSlot slot in skillSlots)
        {
            slot.skillButton.onClick.AddListener(() => CheckAvalablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }

    private void CheckAvalablePoints(SkillSlot slot)
    {
        if (availablePoints > 0)
        {
            slot.TryUpgradeSkill();
        }
    }

    private void HandleAbilityPointsSpend(SkillSlot skillSlot)
    {
        if(availablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
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
        pointsText.text = "Points: " + availablePoints.ToString();
    }
}
