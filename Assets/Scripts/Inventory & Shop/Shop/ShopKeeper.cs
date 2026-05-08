using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;

    public Animator anim;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;

    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmor;

    [SerializeField] private Camera shopkeeperCam;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -1);

    public static event Action<ShopManager, bool> OnShopStateChanged;
    //Use bool here to ind  icate whether it's open or closed
    private bool playerInRange;
    private bool isShopOpen;
    void Start()
    {
        shopkeeperCam = GameManager.Instance.shopCamera;
        shopCanvasGroup = GameManager.Instance.canvasGroup;
        shopManager = GameManager.Instance.shopManager;
    }

    void Update()
    {
        if(playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!isShopOpen)
                {
                    Debug.Log("Button pressed! Shop opened!");
                    Time.timeScale = 0; // Pause the game when the shop is open
                    currentShopKeeper = this; // Set the current shopkeeper reference
                    isShopOpen = true;
                    OnShopStateChanged?.Invoke(shopManager, true);
                    shopCanvasGroup.alpha = 1;
                    shopCanvasGroup.blocksRaycasts = true;
                    shopCanvasGroup.interactable = true;

                    shopkeeperCam.transform.position = transform.position + cameraOffset; 
                    shopkeeperCam.gameObject.SetActive(true);

                    OpenItemShop();
                    // Default to opening the item shop, you can change this to open different shops based on your design
                }
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                Debug.Log("Button pressed once more! Shop closed!");
                Time.timeScale = 1; // Resume the game when the shop is closed
                currentShopKeeper = null; // Clear the current shopkeeper reference
                isShopOpen = false;
                OnShopStateChanged?.Invoke(shopManager, false);
                shopCanvasGroup.alpha = 0;
                shopCanvasGroup.blocksRaycasts = false;
                shopCanvasGroup.interactable = false;

                shopkeeperCam.gameObject.SetActive(false);

            }
        }
    }

    public void OpenItemShop()
    {
        shopManager.PopulateShopItems(shopItems);
    }

    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItems(shopWeapons);
    }

    public void OpenArmorShop()
    {
        shopManager.PopulateShopItems(shopArmor);
    }   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
