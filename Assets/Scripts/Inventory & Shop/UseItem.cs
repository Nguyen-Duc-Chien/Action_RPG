using System.Collections;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffects(ItemSO itemSO)
    {
        if (itemSO.currentHealth > 0)
            StatsManager.Instance.UpdateHealth(itemSO.currentHealth);
        Debug.Log($"Applied {itemSO.currentHealth} health from {itemSO.itemName}");

        if (itemSO.maxHealth > 0)
            StatsManager.Instance.UpdateMaxHealth(itemSO.maxHealth);
        Debug.Log($"Applied {itemSO.maxHealth} max health from {itemSO.itemName}");

        if (itemSO.speed > 0)
            StatsManager.Instance.UpdateSpeed(itemSO.speed);
        Debug.Log($"Applied {itemSO.speed} speed from {itemSO.itemName}");  

        if (itemSO.skillPoints > 0)
        {
            SkillTreeManager skillTree = FindAnyObjectByType<SkillTreeManager>();
            if (skillTree != null)
            {
                skillTree.UpdateAbilityPoints(itemSO.skillPoints);
                Debug.Log($"Applied {itemSO.skillPoints} skill points from {itemSO.itemName}");
            }
        }

        if (itemSO.duration > 0)
            StartCoroutine(EffectTimer(itemSO, itemSO.duration));
    }

    private IEnumerator EffectTimer(ItemSO itemSO, float duration)
    {
        yield return new WaitForSeconds(duration);

        /*if (itemSO.currentHealth > 0)
            StatsManager.Instance.UpdateHealth(-itemSO.currentHealth);
        Debug.Log($"Removed {itemSO.currentHealth} health from {itemSO.itemName}");*/

        /*if (itemSO.maxHealth > 0)
            StatsManager.Instance.UpdateMaxHealth(-itemSO.maxHealth);
        Debug.Log($"Removed {itemSO.maxHealth} max health from {itemSO.itemName}");*/

        if (itemSO.speed > 0)
            StatsManager.Instance.UpdateSpeed(-itemSO.speed);
        Debug.Log($"Removed {itemSO.speed} speed from {itemSO.itemName}");
    }
}
