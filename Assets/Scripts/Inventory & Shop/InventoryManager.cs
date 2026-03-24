using TMPro;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class InventoryManager : MonoBehaviour 
{
    public InventorySlot[] itemSlots;
    public int gold;
    public TMP_Text goldText;

    private void Start()
    {
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        Debug.Log($"Added {quantity} x {itemSO.itemName} to inventory.");
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }
        else
        {
           foreach (var slot in itemSlots)
           {
                if(slot.itemSO == null)
                {
                    slot.itemSO = itemSO;
                    slot.quantity = quantity;
                    slot.UpdateUI();
                    return;
                }
           }
        }
    }
}
