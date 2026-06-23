using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    

    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private InventoryManager inventoryManager;


    public void PopulateShopItems(List<ItemSO> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ItemSO shopItem = shopItems[i];
            shopSlots[i].Initialized(shopItem);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO)
    {
        if(itemSO != null && itemSO.canBuy && inventoryManager.gold >= itemSO.buyPrice)
        {
            if(HasSpaceForItem(itemSO))
            {
                inventoryManager.gold -= itemSO.buyPrice;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                inventoryManager.AddItem(itemSO, 1);
                inventoryManager.SaveToPlayerPrefs();
                Debug.Log("Bought 1 x " + itemSO.itemName);
            }
            else
            {
                 Debug.LogWarning("Not enough inventory space to buy " + itemSO.itemName);
            }
        }
        else // Debugging messages for why the purchase failed
        {
            if(itemSO == null)
                Debug.LogWarning("Item does not exist in shop.");
            else if(!itemSO.canBuy)
                Debug.LogWarning("Item cannot be bought.");
            else if(inventoryManager.gold < itemSO.buyPrice)
                Debug.LogWarning("Not enough gold to buy " + itemSO.itemName);
        }
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.itemSlots)
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                return true;
            else if (slot.itemSO == null)
                return true;
        }
        return false;
    }

    public void SellItem(ItemSO itemSO)
    {
        if (itemSO == null || !itemSO.canSell)
            return;
            
        bool hasItem = false;
        foreach (var slot in inventoryManager.itemSlots)
        {
            if (slot.itemSO == itemSO && slot.quantity > 0)
            {
                hasItem = true;
                slot.quantity--;
                if(slot.quantity <= 0)
                {
                    slot.itemSO = null;
                }
                slot.UpdateUI();
                break;
            }
        }

        if (hasItem)
        {
            inventoryManager.gold += itemSO.sellPrice;
            inventoryManager.goldText.text = inventoryManager.gold.ToString();
            inventoryManager.SaveToPlayerPrefs();
            Debug.Log("Sold 1 x " + itemSO.itemName);
        }
        else
        {
            Debug.LogWarning("Cannot sell " + itemSO.itemName + " because you don't have it in your inventory.");
        }
    }
}
