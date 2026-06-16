using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
            //Column 1
            case "Speed Boost 1":
                StatsManager.Instance.UpdateSpeed(0.5f);
                //Debug.Log("Speed Boost 1 applied.");
                break;
            case "Weapon Range Boost":
                StatsManager.Instance.weaponRange += 1;
                //Debug.Log("Weapon Range Boost applied.");
                break;
            case "Speed Boost 2":
                StatsManager.Instance.UpdateSpeed(0.5f);
                //Debug.Log("Speed Boost 2 applied.");
                break;
            case "Stun Strike":
                StatsManager.Instance.stunTime += 0.2f;
                //Debug.Log("Stun Strike applied.");
                break;

            //Column 2
            case "Health_MaxHPBoost_1":
                StatsManager.Instance.UpdateMaxHealth(1);
                //Debug.Log("Health_MaxHPBoost_1 applied.");
                break;
            case "Health_Resistance":   // Decrease damage taken when your health is below 25% by 50%
                StatsManager.Instance.hasLowHealthResistanceSkill = true;
                //Debug.Log("Health_Resistance unlocked.");
                break;
            case "Health_MaxHPBoost_2":
                StatsManager.Instance.UpdateMaxHealth(1);
                //Debug.Log("Health_MaxHPBoost_2 applied.");
                break;
            case "Health_Regeneration": // Regenerate 2 HP every 5 seconds when your health is below 50%
                StatsManager.Instance.hasHealthRegenSkill = true;
                //Debug.Log("Health_Regeneration unlocked.");
                break;

            //Column 3
            case "Melee Damage Up 1":
                StatsManager.Instance.UpdateMeleeDamage(0.5f);
                //Debug.Log("Melee Damage Up 1 applied.");
                break;
            case "Knockback Force 1":
                StatsManager.Instance.knockbackForce += 0.5f;
                StatsManager.Instance.knockbackTime += 0.05f;
                //Debug.Log("Knockback Force 1 applied.");
                break;
            case "Melee Damage Up 2":
                StatsManager.Instance.UpdateMeleeDamage(0.5f);
                //Debug.Log("Melee Damage Up 2 applied.");
                break;
            case "Knockback Force 2":
                StatsManager.Instance.knockbackForce += 0.5f;
                StatsManager.Instance.knockbackTime += 0.05f;
                //Debug.Log("Knockback Force 2 applied.");
                break;


            //Column 4
            case "Unlock Bow":  // Allow player to use bow and arrow, unlocking the rest of the column
                StatsManager.Instance.isBowUnlocked = true;
                //Debug.Log("Bow has been unlocked!");
                break;
            case "Ranged Damage Up 1":
                StatsManager.Instance.UpdateRangeDamage(0.5f);
                //Debug.Log("Ranged Damage Up 1 applied.");
                break;
            case "Ranged Damage Up 2":
                StatsManager.Instance.UpdateRangeDamage(0.5f);
                //Debug.Log("Ranged Damage Up 2 applied.");
                break;
            case "Rapid Shot":  // Increases the aimingRange of the bow by 1
                Player_Bow bow = FindAnyObjectByType<Player_Bow>(FindObjectsInactive.Include);
                if (bow != null)
                {
                    bow.autoAimRadius += 1.5f;
                    bow.shootCooldown -= 0.2f;
                }
                //Debug.Log("Rapid Shot applied.");
                break;

            //Column 5
            case "Slowness Arrow 1":    // Having a 70% chance to decrease enemy movement speed by 20% for 2 seconds when hit with an arrow
                StatsManager.Instance.arrowSlowChance = 0.7f;
                //Debug.Log("Slowness Arrow 1 applied.");
                break;
            case "Slowness Arrow 2":    // Extra 30% chance to decrease enemy movement speed by 20% for 2 seconds when hit with an arrow
                StatsManager.Instance.arrowSlowChance += 0.3f;
                //Debug.Log("Slowness Arrow 2 applied.");
                break;
            case "Freeze Arrow 1":      // Having a 25% chance to freeze enemies for 2 second when hit with an arrow
                StatsManager.Instance.arrowFreezeChance = 0.15f;
                //Debug.Log("Freeze Arrow 1 applied.");
                break;
            case "Freeze Arrow 2":      // Extra 25% chance to freeze enemies and increase freeze duration to 2 seconds when hit with an arrow
                StatsManager.Instance.arrowFreezeChance += 0.15f;
                //Debug.Log("Freeze Arrow 2 applied.");
                break;

            //Column 6
            case "Burning Arrow":       // Having a 50% chance to set enemies on fire for 2 seconds when hit with an arrow, dealing 1 damage per second
                StatsManager.Instance.arrowBurnChance = 0.7f;
                StatsManager.Instance.arrowBurnDuration = 2f;
                StatsManager.Instance.arrowBurnDamage = 1f;
                Debug.Log("Burning Arrow applied.");
                break;
            case "Knockback Arrow":     
                StatsManager.Instance.arrowKnockbackForce += 0.5f;
                StatsManager.Instance.arrowKnockbackTime += 0.1f;
                Debug.Log("Knockback Arrow applied to Ranged Stats.");
                break;
            case "Ranged Damage Up":    // Increase damage of arrows 0.5f
                StatsManager.Instance.UpdateRangeDamage(0.5f);
                Debug.Log("Ranged Damage Up applied.");
                break;
            case "Extra DoT":           // Extra 30% to set enemies on fire and increase burn duration of arrows by 1 seconds
                StatsManager.Instance.arrowBurnChance += 0.3f;
                StatsManager.Instance.arrowBurnDuration += 1f;
                Debug.Log("Extra DoT applied.");
                break;

            default:
                Debug.LogWarning("Unknown skill: " + skillName);
                break;
        }
    }
}
