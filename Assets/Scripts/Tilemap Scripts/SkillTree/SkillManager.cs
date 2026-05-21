using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player_Combat combat;
    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpend += HandleAbilityPointSpend;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpend -= HandleAbilityPointSpend;
    }

    private void HandleAbilityPointSpend(SkillSlot slot)
    {
        string skillName = slot.skillSO.skillName;

        switch (skillName)
        {
            case "Health1_MaxHPBoost" or "Health3_MaxHPBoost":
                StatsManager.Instance.UpdateMaxHealth(1);
                break;

            case "Sword Slash":
                combat.enabled = true;
                break;

            default:
                Debug.LogWarning("Unknown skill: " + skillName);
                break;
        }
    }
}
