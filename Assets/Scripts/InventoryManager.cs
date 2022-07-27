using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    // Inventory objetcs
    [SerializeField] private GameObject inventoryExitButton;
    [SerializeField] private Animator InventoryAnim;
    [SerializeField] private GameObject invItemPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject ESignPrefab; // Equipped item sign

    // ItemView objects
    [SerializeField] private GameObject itemView;
    [SerializeField] private Image itemPreviewImage;
    [SerializeField] private Text itemDescriptionText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button dropButton;

    // Logic
    [SerializeField] private Player player;
    [SerializeField] private int inventorySpace;
    [SerializeField] private Item[] items; // all items
    [SerializeField] private int lastItem; // Last item in the items array; Number of items

    // Equipped items' index in the items array
    private int weaponEquippedIndex;
    private int armorEquippedIndex;
    private int helmetEquippedIndex;


    public void Awake()
    {
        instance = this;
        items = new Item[inventorySpace];
        lastItem = 0;
        weaponEquippedIndex = -1;
        armorEquippedIndex = -1;
        helmetEquippedIndex = -1;

        // Create the item list in the inventory and its children
        for (int i = 1; i <= inventorySpace; i++)
        {
            GameObject item = Instantiate(invItemPrefab, content);
            item.name = "Item " + i.ToString();
            GameObject ESign = Instantiate(ESignPrefab, item.transform);
            ESign.SetActive(false);
            ESign.name = "ESign";
        }
    }

    // Add item to inventory on pickup
    public void AddItem(Item item)
    {
        if (lastItem < inventorySpace) {  // Inventory isn't full
            item.isEquipped = false;
            items[lastItem] = item;
            lastItem++;
        }
    }

    // Remove item from inventory on drop/sell
    public void RemoveItem(Item item)
    {

    }

    // Opening/closing inventory
    public void OnInventoryClick()
    {
        if (InventoryAnim.GetCurrentAnimatorStateInfo(0).IsName("Inventory_closed"))
        {
            UpdateInventory();
            InventoryAnim.SetTrigger("open");
            itemView.SetActive(false);
        }

        if (InventoryAnim.GetCurrentAnimatorStateInfo(0).IsName("Inventory_opened"))
        {
            InventoryAnim.SetTrigger("close");
            // new WaitForSeconds(1);

        }
    }

    // Displays item on ItemView of given index in the items array
    public void OnItemClick(int index)
    {
        itemView.SetActive(true);

        // Change item sprite and description
        itemPreviewImage.sprite = items[index].itemSprite;

        switch (items[index].type)
        {
            case "Weapon":
                Weapon weapon = (Weapon)items[index];
                itemDescriptionText.text += "Damage: " + weapon.minDmg + "-" + weapon.maxDmg + "\n" +
                "Range: " + weapon.range;
                break;
            case "Helmet":
                Helmet helmet = (Helmet)items[index];
                itemDescriptionText.text += "Defense: " + helmet.defense + "\n";
                break;
            case "Legs":
                break;
            case "Armor":
                Armor armor = (Armor)items[index];
                itemDescriptionText.text += "Defense: " + armor.defense + "\n";
                break;
        }

        itemDescriptionText.text = "Required Lvl: " + items[index].requiredLvl + "\n";
        if (player.GetLevel() < items[index].requiredLvl)
            itemDescriptionText.color = Color.red;
        else itemDescriptionText.color = Color.white;

        // Create equip/drop events according to item click
        equipButton.onClick.RemoveAllListeners(); // To clear other listeners on this button
        equipButton.onClick.AddListener(() => OnEquipClick(index));
        dropButton.onClick.AddListener(() => OnDropClick(index));

        // Change text to unequipped if item is equipped already
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        if (items[index].isEquipped == false)
            equipButtonText.text = "Equip";
        else equipButtonText.text = "Unequip";

    }

    // Equiping/unequipping an item of given index in the items array
    public void OnEquipClick(int index)
    {
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        GameObject currentESign = content.transform.GetChild(index).Find("ESign").gameObject;
        //GameObject currentESignWeapon = content.transform.GetChild(index).Find("ESign").gameObject;

        // Equip
        if (items[index].isEquipped == false)
        {
            if (player.GetLevel() >= items[index].requiredLvl)
            {
                switch (items[index].type)
                {
                    case "Weapon":
                        // Unequip the equipped weapon
                        if (weaponEquippedIndex != -1)
                        {
                            items[weaponEquippedIndex].isEquipped = false;
                            GameObject lastESignWeapon = content.transform.GetChild(weaponEquippedIndex).Find("ESign").gameObject;
                            lastESignWeapon.SetActive(false);
                        }
                        weaponEquippedIndex = index; // Assign index of equipped weapon                                               
                        player.transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = items[index].itemSprite; // Change sprite
                        break;

                    case "Armor":
                        // Unequip the equipped armor
                        if (armorEquippedIndex != -1)
                        {
                            items[armorEquippedIndex].isEquipped = false;
                            GameObject lastESignArmor = content.transform.GetChild(armorEquippedIndex).Find("ESign").gameObject;
                            lastESignArmor.SetActive(false);
                        }
                        armorEquippedIndex = index; // Assign index of equipped armor 
                        player.transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = items[index].itemSprite;
                        break;

                    case "Helmet":
                        // Unequip the equipped helmet
                        if (helmetEquippedIndex != -1)
                        {
                            items[helmetEquippedIndex].isEquipped = false;
                            GameObject lastESignHelmet = content.transform.GetChild(helmetEquippedIndex).Find("ESign").gameObject;
                            lastESignHelmet.SetActive(false);
                        }
                        helmetEquippedIndex = index; // Assign index of equipped helmet
                        player.transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = items[index].itemSprite;
                        break;
                }
            }
            else
            {
                Debug.Log("Required lvl not met!");
                return;
                    }

            currentESign.SetActive(true);
            items[index].isEquipped = true;
            equipButtonText.text = "Unequip";
            Debug.Log("Equipped item: " + items[index]);
        }

        // Unequip
        else
        {
            switch (items[index].type)
            {
                case "Weapon":
                    player.transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = null;
                    break;
                case "Armor":
                    player.transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = null;
                    break;
                case "Helmet":
                    player.transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = null;
                    break;
            }
            currentESign.SetActive(false);
            items[index].isEquipped = false;
            equipButtonText.text = "Equip";
            Debug.Log("Unequipped item: " + items[index]);
        }
    }
    public void OnDropClick(int i)
    {

    }

    // Update inventory information
    public void UpdateInventory()
    {
        for (int i = 0; i < lastItem; i++)
        {
            Text itemName = content.transform.GetChild(i).Find("ItemName").GetComponent<Text>();
            Image itemImage = content.transform.GetChild(i).Find("ItemImage").GetComponent<Image>();
            Button itemButton = content.transform.GetChild(i).GetComponent<Button>();

            itemName.text = items[i].itemName;
            itemImage.sprite = items[i].inverntoryIcon;
            itemImage.color = Color.white;


            int temp = i;
            itemButton.onClick.AddListener(() => OnItemClick(temp));
        }
    }

    public bool isFull()
    {
        return lastItem >= inventorySpace;
    }
}
