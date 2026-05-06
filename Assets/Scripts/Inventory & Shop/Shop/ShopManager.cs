using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    

    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private InventoryManager inventoryManager;


    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialized(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        if(itemSO != null && inventoryManager.gold >= price)
        {
            if(HasSpaceForItem(itemSO))
            {
                inventoryManager.gold -= price;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                inventoryManager.AddItem(itemSO, 1);
                Debug.Log("Bought 1 x " + itemSO.itemName);
            }
            else
            {
                 Debug.LogWarning("Not enough inventory space to buy " + itemSO.itemName);
            }
        }
        else // Debugging messages for why the purchase failed
        {
            if(inventoryManager.gold < price)
                Debug.LogWarning("Not enough gold to buy " + itemSO.itemName);
            if(itemSO == null)
                Debug.LogWarning("Item does not exist in shop.");
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
        if (itemSO == null)
            return;
        foreach (var slot in shopSlots)
        {
            if(slot.itemSO == itemSO)
            {
                inventoryManager.gold += slot.price;  // Flexable to allow different sell prices in the future
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                return;
            }
        }
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}
