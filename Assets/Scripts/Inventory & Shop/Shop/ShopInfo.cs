using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;

    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    [Header("Stat Fields")]
    public TMP_Text[] statTexts;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
    }

    public void ShowItemInfo(ItemSO itemSO)     //Display Item Info
    {
        infoPanel.alpha = 1;

        itemNameText.text = itemSO.itemName;
        itemDescriptionText.text = itemSO.itemDescription;

        List<string> stats = new List<string>();
        if (itemSO.currentHealth > 0) stats.Add("Health: " + itemSO.currentHealth.ToString());
        if (itemSO.maxHealth > 0) stats.Add("Max Health: " + itemSO.maxHealth.ToString());
        if (itemSO.meleeDamage > 0) stats.Add("Melee Damage: " + itemSO.meleeDamage.ToString());
        if (itemSO.rangeDamage > 0) stats.Add("Range Damage: " + itemSO.rangeDamage.ToString());
        if (itemSO.speed > 0) stats.Add("Speed: " + itemSO.speed.ToString());
        if (itemSO.skillPoints > 0) stats.Add("Skill Points: " + itemSO.skillPoints.ToString());
        if (itemSO.duration > 0) stats.Add("Duration: " + itemSO.duration.ToString());
        for (int i = 0; i < statTexts.Length; i++)
        {
            if(i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideItemInfo(ItemSO itemSO)     //Hides the Panel
    {
        infoPanel.alpha = 0;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void FollowMouse()                   //Makes the Panel follow the mouse
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(200, -150, 0);
        //Note: If you want your panel to have a fixed position,
        //you can leave out the follow mouse stuff altogether

        infoPanelRect.position = mousePosition + offset;
    }
}
