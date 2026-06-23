using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;

    public bool isConsumable;

    public bool isGold;
    public int stackSize = 16;

    [Header("Shop")]
    public bool canBuy = true;
    public int buyPrice;
    public bool canSell = true;
    public int sellPrice;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth;
    public float speed;
    public float meleeDamage;
    public float rangeDamage;

    [Header("For Temporary Items")]
    public float duration;
}
