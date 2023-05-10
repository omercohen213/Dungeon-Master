using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    private Inventory inventory;
    private Player player;

    private int[] equippedIndexes;

    // Inventory objetcs
    [SerializeField] private GameObject invItemPrefab;
    [SerializeField] private GameObject ESignPrefab; // Equipped item sign in the inventory
    [SerializeField] private Animator animator;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject inventoryExitButton;

    // ItemView objects
    [SerializeField] private GameObject itemView;
    [SerializeField] private Image itemPreviewImage;
    [SerializeField] private Text itemDescriptionText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button dropButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Player.instance;
        inventory = Inventory.instance;
        equippedIndexes = inventory.EquippedIndexes;

        // Create the item list in the inventory and its children
        for (int i = 0; i <= inventory.InventorySpace; i++)
        {
            GameObject itemButton = Instantiate(invItemPrefab, content);
            itemButton.name = "Item " + i.ToString();
            GameObject ESign = Instantiate(ESignPrefab, itemButton.transform);
            ESign.SetActive(false);
            ESign.name = "ESign";
        }
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

    // Update inventory information
    public void UpdateInventory()
    {
        for (int i = 0; i < inventory.InventorySpace; i++)
        {
            Button itemButton = content.transform.GetChild(i).GetComponent<Button>();
            // Remove listeners from buttons with no items 
            if (i > inventory.Items.Count - 1)
            {
                itemButton.onClick.RemoveAllListeners();
            }
            else
            {
                Text itemName = content.transform.GetChild(i).Find("ItemName").GetComponent<Text>();
                Image itemImage = content.transform.GetChild(i).Find("ItemImage").GetComponent<Image>();

                // Set the item object in the inventory view
                itemName.text = inventory.Items[i].itemName;
                itemImage.sprite = inventory.Items[i].inverntoryIcon;
                itemImage.color = Color.white;
                int index = i;
                itemButton.onClick.AddListener(() => OnItemClick(index));
            }
            RevmoveESign(i);
        }

        for (int i = 0; i < equippedIndexes.Length; i++)
        {            
            if (equippedIndexes[i] != -1)
                AddESign(equippedIndexes[i]);
        }
    }

    // Displays item on ItemView of given index
    public void OnItemClick(int index)
    {
        itemView.SetActive(true);
        Item item = inventory.Items[index];

        // Change item sprite and description
        itemPreviewImage.sprite = item.itemSprite;
        itemDescriptionText.text = "";

        switch (item.type)
        {
            case "Weapon":
                Weapon weapon = (Weapon)item;
                itemDescriptionText.text += "Damage: " + weapon.attackPower + "\n" +
                "Range: " + weapon.range;
                break;
            case "Helmet":
                Helmet helmet = (Helmet)item;
                itemDescriptionText.text += "Defense: " + helmet.defense + "\n";
                break;
            case "Armor":
                Armor armor = (Armor)item;
                itemDescriptionText.text += "Defense: " + armor.defense + "\n";
                break;
        }

        string lvlText = "Required Lvl: " + item.requiredLvl + "\n";
        itemDescriptionText.text = lvlText + itemDescriptionText.text;

        if (player.Lvl < item.requiredLvl)
            itemDescriptionText.color = Color.red;
        else itemDescriptionText.color = Color.white;

        // Create new equip/drop listener according to item click
        equipButton.onClick.RemoveAllListeners(); // Clear other listeners on this button
        equipButton.onClick.AddListener(() => OnEquipButtonClick(index));
        dropButton.onClick.RemoveAllListeners();
        dropButton.onClick.AddListener(() => OnDropClick(index));

        // Change text to unequipped if item is equipped already
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        if (!inventory.isEquipped(index))
            equipButtonText.text = "Equip";
        else equipButtonText.text = "Unequip";
    }
    
    // Equiping/unequipping an item of given index
    public void OnEquipButtonClick(int index)
    {
        Item item = inventory.Items[index];

        // Equip
        if (!inventory.isEquipped(index))
        {
            if (player.Lvl >= item.requiredLvl)
                inventory.EquipItem(item, index);
            else
                Debug.Log("Required lvl not met!");
        }
        // Unequip
        else
        {
            if (item.type == "Weapon")
                Debug.Log("You can't unequip your weapon!");
            else
                inventory.UnequipItem(item);
        }
        UpdateInventory();
    }

    // Drop the item on Drop click and update the player's inventory
    public void OnDropClick(int index)
    {
        if (inventory.isEquipped(index))
            Debug.Log("You can't drop an equipped item!");
        else
        {
            ShowEmpty();
            inventory.DropItem(inventory.Items[index], index);
            itemView.SetActive(false);
            UpdateInventory();
        }
    }

    // Show empty instead of the last item in inventory 
    private void ShowEmpty()
    {
        int lastItem = inventory.Items.Count - 1;

        Text itemName = content.transform.GetChild(lastItem).Find("ItemName").GetComponent<Text>();
        Image itemImage = content.transform.GetChild(lastItem).Find("ItemImage").GetComponent<Image>();
        itemName.text = "(Empty)";
        itemImage.sprite = null;
        var tempColor = Color.white;
        tempColor.a = 0;
        itemImage.color = tempColor;
    }

    private void AddESign(int index)
    {
        GameObject ESign = content.transform.GetChild(index).Find("ESign").gameObject;
        ESign.SetActive(true);
    }

    private void RevmoveESign(int index)
    {
        GameObject ESign = content.transform.GetChild(index).Find("ESign").gameObject;
        ESign.SetActive(false);
    }

}

