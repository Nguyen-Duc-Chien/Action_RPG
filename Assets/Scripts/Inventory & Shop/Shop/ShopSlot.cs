using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;   

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text buyPriceText;
    public TMP_Text sellPriceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo;

    public GameObject buyButtonObj;
    public GameObject sellButtonObj;

    public void Initialized(ItemSO newItemSO)
    {
        if (newItemSO == null)
        {
            Debug.LogError($"[ShopSlot] newItemSO truyền vào bị NULL ở ô {gameObject.name}! Hãy kiểm tra lại list trong ShopKeeper xem có phần tử nào đang trống (None) không.");
            return;
        }
        if (itemImage == null)
        {
            Debug.LogError($"[ShopSlot] itemImage bị NULL ở ô {gameObject.name}! Bạn chưa gán Image vào biến itemImage trong Inspector của ô này.");
            return;
        }
        if (itemNameText == null)
        {
            Debug.LogError($"[ShopSlot] itemNameText bị NULL ở ô {gameObject.name}! Bạn chưa gán Text vào biến itemNameText.");
            return;
        }

        itemSO = newItemSO;
        itemImage.sprite = itemSO.icon;
        itemNameText.text = itemSO.itemName;
        
        if (buyPriceText != null)
            buyPriceText.text = "BUY: " + itemSO.buyPrice;
            
        if (sellPriceText != null)
            sellPriceText.text = "SELL: " + itemSO.sellPrice;

        if (buyButtonObj != null) buyButtonObj.SetActive(itemSO.canBuy);
        if (sellButtonObj != null) sellButtonObj.SetActive(itemSO.canSell);
    }

    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(itemSO);
    }

    public void OnSellButtonClicked()
    {
        shopManager.SellItem(itemSO);
    }

    //Kích chuột phải vào chữ IPointerEnterHandler và chọn ...
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSO != null)
            shopInfo.ShowItemInfo(itemSO);
    }

    //Kích chuột phải vào chữ IPointerExitHandler và chọn ...
    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HideItemInfo(itemSO);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSO != null)
            shopInfo.FollowMouse();
    }
}
