using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    // Inventory objetcs
    [SerializeField] private GameObject inventoryExitButton;
    [SerializeField] private Animator animator;
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


    public void Awake()
    {
        instance = this;      
    }
    private void Start()
    {
        // Create the item list in the inventory and its children
        for (int i = 1; i <= inventorySpace + 1; i++)
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
        //player.items.Add(item);
        player.items[player.lastItem] = item;
        player.lastItem++;
        GameManager.instance.SaveGame();
    }

    public bool isFull()
    {
        //return player.items.Count >= inventorySpace + 1;
        return player.lastItem -1 >= inventorySpace;
    }

    // Remove item from inventory on drop/sell
    public void RemoveItem(Item item)
    {

    }

    // Opening/closing inventory
    public void OnInventoryClick()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Inventory_closed"))
        {
            UpdateInventory();
            animator.SetTrigger("open");
            itemView.SetActive(false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Inventory_opened"))
            animator.SetTrigger("close");
    }

    // Displays item on ItemView of given index in the items array
    public void OnItemClick(int index)
    {
        itemView.SetActive(true);

        // Change item sprite and description
        itemPreviewImage.sprite = player.items[index].itemSprite;

        switch (player.items[index].type)
        {
            case "Weapon":
                Weapon weapon = (Weapon)player.items[index];
                itemDescriptionText.text += "Damage: " + weapon.minDmg + "-" + weapon.maxDmg + "\n" +
                "Range: " + weapon.range;
                break;
            case "Helmet":
                Helmet helmet = (Helmet)player.items[index];
                itemDescriptionText.text += "Defense: " + helmet.defense + "\n";
                break;
            case "Legs":
                break;
            case "Armor":
                Armor armor = (Armor)player.items[index];
                itemDescriptionText.text += "Defense: " + armor.defense + "\n";
                break;
        }

        itemDescriptionText.text = "Required Lvl: " + player.items[index].requiredLvl + "\n";
        if (player.lvl < player.items[index].requiredLvl)
            itemDescriptionText.color = Color.red;
        else itemDescriptionText.color = Color.white;

        // Create equip/drop events according to item click
        equipButton.onClick.RemoveAllListeners(); // Clear other listeners on this button
        equipButton.onClick.AddListener(() => OnEquipClick(index));
        dropButton.onClick.AddListener(() => OnDropClick(index));

        // Change text to unequipped if item is equipped already
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        if (index == player.equippedArmorIndex || index == player.equippedHelmetIndex || index == player.equippedWeaponIndex)
            equipButtonText.text = "Unequip";
        else equipButtonText.text = "Equip";
    }

    // Equiping/unequipping an item of given index in the items array
    public void OnEquipClick(int index)
    {
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        GameObject ESign = content.transform.GetChild(index).Find("ESign").gameObject;

        // Equip
        if (!(index == player.equippedArmorIndex) && !(index == player.equippedHelmetIndex) && !(index == player.equippedWeaponIndex))
        {
            if (player.lvl >= player.items[index].requiredLvl)
            {
                switch (player.items[index].type)
                {
                    case "Weapon":
                        // Unequip the equipped weapon
                        if (player.equippedWeaponIndex != -1)
                        {
                            GameObject lastESignWeapon = content.transform.GetChild(player.equippedWeaponIndex).Find("ESign").gameObject;
                            lastESignWeapon.SetActive(false);
                        }
                        player.transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = player.items[index].itemSprite; // Change sprite
                        player.equippedWeaponIndex = index; // Assign index of equipped weapon
                        break;

                    case "Armor":
                        // Unequip the equipped armor
                        if (player.equippedArmorIndex != -1)
                        {
                            GameObject lastESignArmor = content.transform.GetChild(player.equippedArmorIndex).Find("ESign").gameObject;
                            lastESignArmor.SetActive(false);
                        }
                        player.transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = player.items[index].itemSprite;
                        player.equippedArmorIndex = index; // Assign index of equipped armor 
                        break;

                    case "Helmet":
                        // Unequip the equipped helmet
                        if (player.equippedHelmetIndex != -1)
                        {
                            GameObject lastESignHelmet = content.transform.GetChild(player.equippedHelmetIndex).Find("ESign").gameObject;
                            lastESignHelmet.SetActive(false);
                        }
                        
                        player.transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = player.items[index].itemSprite;
                        player.equippedHelmetIndex = index; // Assign index of equipped helmet
                        break;
                }
                GameManager.instance.SaveGame();
            }
            else
            {
                Debug.Log("Required lvl not met!");
                return;
            }

            ESign.SetActive(true);
            equipButtonText.text = "Unequip";
        }

        // Unequip
        else
        {
            if (player.items[index].type == "Weapon")
            {
                Debug.Log("You can't unequip your weapon!");
                return;
            }

            switch (player.items[index].type)
            {
                case "Armor":
                    player.transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = null;
                    player.equippedArmorIndex = -1;
                    break;
                case "Helmet":
                    player.transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = null;
                    player.equippedHelmetIndex = -1;
                    break;
            }
            ESign.SetActive(false);
            equipButtonText.text = "Equip";
            GameManager.instance.SaveGame();
        }
    }
    public void OnDropClick(int i)
    {

    }
    // Update inventory information
    public void UpdateInventory()
    {
        for (int i = 0; i < player.lastItem; i++)
        {
            // Set the item object in the inventory view
            Text itemName = content.transform.GetChild(i).Find("ItemName").GetComponent<Text>();
            Image itemImage = content.transform.GetChild(i).Find("ItemImage").GetComponent<Image>();
            Button itemButton = content.transform.GetChild(i).GetComponent<Button>();
            itemName.text = player.items[i].itemName;
            itemImage.sprite = player.items[i].inverntoryIcon;
            itemImage.color = Color.white;

            int temp = i;
            itemButton.onClick.AddListener(() => OnItemClick(temp));

        }

        // ESign on equipped items
        if (player.equippedWeaponIndex != -1)
            content.transform.GetChild(player.equippedWeaponIndex).Find("ESign").gameObject.SetActive(true);
        if (player.equippedArmorIndex != -1)
            content.transform.GetChild(player.equippedArmorIndex).Find("ESign").gameObject.SetActive(true);
        if (player.equippedHelmetIndex != -1)
            content.transform.GetChild(player.equippedHelmetIndex).Find("ESign").gameObject.SetActive(true);
    }
}
