using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour 
{
    public InventorySlot[] itemSlots;
    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;

    public Transform player;
    private void Start()
    {
        LoadFromSave();
        goldText.text = gold.ToString();
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
        //Debug.Log($"Added {quantity} x {itemSO.itemName} to inventory.");
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            SaveToPlayerPrefs();
            return;
        }
        
        foreach (var slot in itemSlots)     // Try if item can be stacked in existing slot
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int availableSpace = itemSO.stackSize - slot.quantity;      
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();

                if (quantity <= 0)
                    return;
            }
        }
        
        foreach (var slot in itemSlots)    // Other items that can't be stacked right now, find empty slot
        {
            if(slot.itemSO == null)
            {
                int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                slot.itemSO = itemSO;
                slot.quantity = quantity;
                slot.UpdateUI();
                SaveToPlayerPrefs();
                return;
            }
        }

        if(quantity > 0)
        {
            //Debug.LogWarning($"Not enough inventory space to add {quantity} x {itemSO.itemName}.");
            DropLoot(itemSO, quantity);
        }
    }

    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
        SaveToPlayerPrefs();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity);
    }

    public void UseItem(InventorySlot slot)
    {
        if(slot.itemSO != null && slot.quantity >= 0)
        {
            useItem.ApplyItemEffects(slot.itemSO);

            slot.quantity--;
            if(slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
            SaveToPlayerPrefs();
        }
    }

    #region Save/Load

    public void SaveToPlayerPrefs()
    {
        if (RunManager.Instance == null) return;

        RunManager.Instance.SaveGold(gold);
        RunManager.Instance.SaveInventory(itemSlots);
    }

    private void LoadFromSave()
    {
        if (RunManager.Instance == null) return;

        gold = RunManager.Instance.LoadGold();
        RunManager.Instance.LoadInventory(itemSlots);
    }

    #endregion
}
