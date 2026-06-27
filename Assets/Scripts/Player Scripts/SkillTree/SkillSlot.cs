using UnityEngine;
using UnityEngine.UI; // For Image component
using TMPro;
using System;
using NUnit.Framework; // For TextMeshPro component
using System.Collections.Generic; // Add this to use generic List<T>

public class SkillSlot : MonoBehaviour
{
    [Header("Prerequisite Skills")]
    public List<SkillSlot> prerequisiteSkillSlots;
    [Header("Conflict Skills")]
    public List<SkillSlot> conflictingSkillSlots;

    public SkillSO skillSO;

    public int currentLevel;
    public bool isUnlocked;

    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;

    public static event Action<SkillSlot> OnAbilityPointSpend;
    public static event Action<SkillSlot> OnSkillMaxed;

    /// <summary>
    /// Dùng khi load save — fire event để SkillManager re-apply effect mà không trừ point.
    /// </summary>
    public static void InvokeAbilityPointSpendForLoad(SkillSlot slot)
    {
        OnAbilityPointSpend?.Invoke(slot);
    }

    private void OnValidate()
    {
        if (skillSO != null && skillLevelText != null)
        {
            UpdateUI();
        }
    }

    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSO.maxLevel && CanUnlockSkill())
        {
            currentLevel++;
            UpdateUI();
            OnAbilityPointSpend?.Invoke(this);
            if (currentLevel >= skillSO.maxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }
        }
    }

    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked || slot.currentLevel < slot.skillSO.maxLevel)
            {
                return false;
            }
        }

        foreach (SkillSlot slot in conflictingSkillSlots)
        {
            if (slot.currentLevel > 0)
            {
                return false;
            }
        }

        return true;
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (skillIcon == null || skillButton == null || skillLevelText == null) return;

        skillIcon.sprite = skillSO.skillIcon;

        if (isUnlocked && CanUnlockSkill())
        {
            skillButton.interactable = true;
            skillLevelText.text = currentLevel.ToString() + " / " + skillSO.maxLevel.ToString();
            skillIcon.color = Color.white;
        }
        else
        {
            skillButton.interactable = false;

            if (isUnlocked && !CanUnlockSkill())
            {
                skillLevelText.text = "Blocked";
                skillIcon.color = new Color(0.3f, 0.3f, 0.3f, 0.6f); 
            }
            else
            {
                skillLevelText.text = "Locked";
                skillIcon.color = Color.grey;
            }
        }
    }
}
