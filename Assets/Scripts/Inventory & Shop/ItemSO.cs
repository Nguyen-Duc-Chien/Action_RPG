using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;

    public bool isGold;
    public int stackSize = 16;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth;
    public float speed;
    public float meleeDamage;
    public float rangeDamage;

    [Header("For Temporary Items")]
    public float duration;
}
