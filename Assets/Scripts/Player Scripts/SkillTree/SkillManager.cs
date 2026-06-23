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
            case "Speed Boost 1":       // Increase player movement speed
                StatsManager.Instance.UpdateSpeed(0.5f);
                //Debug.Log("Speed Boost 1 applied.");
                break;
            case "Weapon Range Boost":  // Increase the range of melee attacks
                StatsManager.Instance.weaponRange += 1;
                //Debug.Log("Weapon Range Boost applied.");
                break;
            case "Speed Boost 2":       // Increase player movement speed for more mobility
                StatsManager.Instance.UpdateSpeed(0.5f);
                //Debug.Log("Speed Boost 2 applied.");
                break;
            case "Extra Energy":        // Increase max energy for more energy
                if (EnergyManager.Instance != null)
                {
                    EnergyManager.Instance.UpdateMaxEnergy(10f);
                }
                //Debug.Log("Extra Energy applied.");
                break;

            //Column 2
            case "Health_MaxHPBoost_1":     // Increase max health
                StatsManager.Instance.UpdateMaxHealth(1);
                //Debug.Log("Health_MaxHPBoost_1 applied.");
                break;
            case "Health_Resistance":   // Decrease damage taken when your health is so low
                StatsManager.Instance.hasLowHealthResistanceSkill = true;
                //Debug.Log("Health_Resistance unlocked.");
                break;
            case "Health_MaxHPBoost_2":     // Increase max health for more survivability
                StatsManager.Instance.UpdateMaxHealth(1);
                //Debug.Log("Health_MaxHPBoost_2 applied.");
                break;
            case "Health_Regeneration": // Regenerate HP when your health is low
                StatsManager.Instance.hasHealthRegenSkill = true;
                //Debug.Log("Health_Regeneration unlocked.");
                break;

            //Column 3
            case "Melee Damage Up 1":   // Increase melee damage
                StatsManager.Instance.UpdateMeleeDamage(0.5f);
                //Debug.Log("Melee Damage Up 1 applied.");
                break;
            case "Knockback Force 1":   // Increase knockback force and knockback duration of melee attacks
                StatsManager.Instance.knockbackForce += 0.5f;
                StatsManager.Instance.knockbackTime += 0.05f;
                //Debug.Log("Knockback Force 1 applied.");
                break;
            /*case "Melee Damage Up 2":   // Increase melee damage for more damage output
                StatsManager.Instance.UpdateMeleeDamage(0.5f);
                //Debug.Log("Melee Damage Up 2 applied.");
                break;
            case "Knockback Force 2":   // Increase knockback force and knockback duration of melee attacks for more
                StatsManager.Instance.knockbackForce += 0.5f;
                StatsManager.Instance.knockbackTime += 0.05f;
                //Debug.Log("Knockback Force 2 applied.");
                break;*/
            case "Stun Strike":         // Increase the duration of the stun effect on enemies when hit with melee attacks
                StatsManager.Instance.stunTime += 0.2f;
                //Debug.Log("Stun Strike applied.");
                break;
            case "Dash Efficiency":     // Decrease the energy cost of dashing
                PlayerDash dash = FindAnyObjectByType<PlayerDash>(FindObjectsInactive.Include);
                if (dash != null)
                {
                    dash.dashEnergyCost -= 5f;
                }
                //Debug.Log("Dash Efficiency applied.");
                break;

            //Column 4
            case "Unlock Bow":          // Allow player to use bow and arrow, unlocking the rest of the column
                StatsManager.Instance.isBowUnlocked = true;
                //Debug.Log("Bow has been unlocked!");
                break;
            case "Ranged Damage Up 1":  // Increase damage of arrows
                StatsManager.Instance.UpdateRangeDamage(0.5f);
                //Debug.Log("Ranged Damage Up 1 applied.");
                break;
            case "Ranged Damage Up 2":  // Increase damage of arrows
                StatsManager.Instance.UpdateRangeDamage(0.5f);
                //Debug.Log("Ranged Damage Up 2 applied.");
                break;
            case "Rapid Shot":          // Increases the aim radius and decreases the cooldown of the bow, allowing for faster shooting
                Player_Bow bow = FindAnyObjectByType<Player_Bow>(FindObjectsInactive.Include);
                if (bow != null)
                {
                    bow.autoAimRadius += 1.5f;
                    bow.shootCooldown -= 0.2f;
                }
                //Debug.Log("Rapid Shot applied.");
                break;

            //Column 5
            case "Slowness Arrow 1":    // Have a small chance to decrease enemy movement speed when hit with an arrow
                StatsManager.Instance.arrowSlowChance = 0.5f;
                //Debug.Log("Slowness Arrow 1 applied.");
                break;
            case "Slowness Arrow 2":    // Increase the chance to decrease enemy movement speed when hit with an arrow
                StatsManager.Instance.arrowSlowChance += 0.25f;
                //Debug.Log("Slowness Arrow 2 applied.");
                break;
            case "Freeze Arrow 1":      // Have a tiny chance to freeze enemies when hit with an arrow
                StatsManager.Instance.arrowFreezeChance = 0.15f;
                //Debug.Log("Freeze Arrow 1 applied.");
                break;
            case "Freeze Arrow 2":      // Increase the chance to freeze enemies and increase freeze duration when hit with an arrow
                StatsManager.Instance.arrowFreezeChance += 0.15f;
                //Debug.Log("Freeze Arrow 2 applied.");
                break;

            //Column 6
            case "Burning Arrow":       // Have a small chance to set enemies on fire when hit with an arrow, dealing burn DoT
                StatsManager.Instance.arrowBurnChance = 0.5f;
                StatsManager.Instance.arrowBurnDuration = 2f;
                StatsManager.Instance.arrowBurnDamage = 1f;
                Debug.Log("Burning Arrow applied.");
                break;
            case "Knockback Arrow":     // Increase knockback force and knockback duration of arrows
                StatsManager.Instance.arrowKnockbackForce += 0.5f;
                StatsManager.Instance.arrowKnockbackTime += 0.1f;
                Debug.Log("Knockback Arrow applied to Ranged Stats.");
                break;
            case "Ranged Damage Up":    // Increase damage of arrows
                StatsManager.Instance.UpdateRangeDamage(0.5f);
                Debug.Log("Ranged Damage Up applied.");
                break;
            case "Extra DoT":           // Increase the chance to set enemies on fire and increase DoT burn duration of arrows
                StatsManager.Instance.arrowBurnChance += 0.25f;
                StatsManager.Instance.arrowBurnDuration += 1f;
                Debug.Log("Extra DoT applied.");
                break;

            default:
                Debug.LogWarning("Unknown skill: " + skillName);
                break;
        }
    }
}
