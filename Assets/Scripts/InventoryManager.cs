using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] private Player player;

    [SerializeField] private GameObject ItemView;
    [SerializeField] private Image itemPreviewImage;
    [SerializeField] private GameObject inventoryExitButton;
    [SerializeField] private Animator InventoryAnim;
    [SerializeField] private GameObject inventoryItem;
    [SerializeField] private Transform content;

    [SerializeField] private int inventorySpace;
    [SerializeField] private Item[] items;
    [SerializeField] private int lastItem;

    public void Awake()
    {
        instance = this;
        items = new Item[inventorySpace];
        lastItem = 0;
        for (int i = 1; i <= inventorySpace; i++)
        {
            GameObject item = Instantiate(inventoryItem, content);
            item.name = "Item " + i.ToString();
        }
    }

    public void Add(Item item)
    {
        if (lastItem >= inventorySpace)
            Debug.Log("No space");
        else
        {
            items[lastItem] = item;
            lastItem++;
        }
    }

    public void Remove(Item item)
    {

    }

    public void OnInventoryClick()
    {
        if (InventoryAnim.GetCurrentAnimatorStateInfo(0).IsName("Inventory_closed"))
        {
            UpdateInventory();
            InventoryAnim.SetTrigger("open");
            //inventoryExitButton.SetActive(true);
            ItemView.SetActive(false);
        }

        if (InventoryAnim.GetCurrentAnimatorStateInfo(0).IsName("Inventory_opened"))
        {
            InventoryAnim.SetTrigger("close");
            // new WaitForSeconds(1);
            //inventoryExitButton.SetActive(false);

        }
    }

    public void OnItemClick(int i)
    {
        ItemView.SetActive(true);
        Debug.Log(i);
        itemPreviewImage.sprite = items[i].inverntorySprite;

    }

    // Update character information
    public void UpdateInventory()
    {
        // Weapon
        //weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLvl];    
        for (int i = 0; i < inventorySpace; i++)
        {
            if (items[i] != null) {
               Text itemName = content.transform.GetChild(i).Find("ItemName").GetComponent<Text>();
               Image itemImage = content.transform.GetChild(i).Find("ItemImage").GetComponent<Image>();
               Button itemButton = content.transform.GetChild(i).GetComponent<Button>();
               itemName.text = items[i].itemName;
               itemImage.sprite = items[i].inverntorySprite;
               itemImage.color = Color.white;
               itemButton.onClick.AddListener(() => OnItemClick(i));
            }
        }
    }
}
