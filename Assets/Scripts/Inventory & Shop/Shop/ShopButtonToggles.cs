using UnityEngine;

public class ShopButtonToggles : MonoBehaviour
{
    public void OpenConsumablesShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenConsumablesShop();
        }
    }

    public void OpenMiscellaneousShop()
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenMiscellaneousShop();
        }
    }
}
