using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemSO> allItems;

    public ItemSO GetItemByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;

        foreach (var item in allItems)
        {
            if (item != null && item.itemName == itemName)
                return item;
        }

        Debug.LogWarning($"[ItemDatabase] Item '{itemName}' not found in database!");
        return null;
    }
}
