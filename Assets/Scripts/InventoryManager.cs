using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

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

    // Drop object
    [SerializeField] private GameObject itemDropPrefab;

    // Logic
    private Player player;
    private int lastItem; // Number of items the player has
    private int equippedWeaponIndex;
    private int equippedArmorIndex;
    private int equippedHelmetIndex;
    private int inventorySpace = 14;

    public int LastItem { get => lastItem; set => lastItem = value; }
    public int EquippedWeaponIndex { get => equippedWeaponIndex; set => equippedWeaponIndex = value; }
    public int EquippedArmorIndex { get => equippedArmorIndex; set => equippedArmorIndex = value; }
    public int EquippedHelmetIndex { get => equippedHelmetIndex; set => equippedHelmetIndex = value; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Player.instance;
        // Create the item list in the inventory and its children
        for (int i = 0; i <= inventorySpace; i++)
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
        player.Items[LastItem] = item;
        LastItem++;
        GameManager.instance.SaveGame();
    }

    public bool isFull()
    {
        return LastItem - 1 >= inventorySpace;
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
        Item item = player.Items[index];

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
        equipButton.onClick.AddListener(() => OnEquipClick(index));
        dropButton.onClick.RemoveAllListeners();
        dropButton.onClick.AddListener(() => OnDropClick(index));

        // Change text to unequipped if item is equipped already
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        if (isEquipped(index))
            equipButtonText.text = "Unequip";
        else equipButtonText.text = "Equip";
    }

    // Equiping/unequipping an item of given index in the items array
    public void OnEquipClick(int index)
    {
        Text equipButtonText = equipButton.GetComponentInChildren<Text>();
        GameObject ESign = content.transform.GetChild(index).Find("ESign").gameObject;
        Item item = player.Items[index];

        // Equip
        if (!isEquipped(index))
        {
            if (player.Lvl >= item.requiredLvl)
            {
                UnequipEquippedItem(item);
                EquipChosenItem(item, index);
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
            if (item.type == "Weapon") {
                Debug.Log("You can't unequip your weapon!");
            }
            else
            {
                UnequipEquippedItem(item);
                ESign.SetActive(false);
                equipButtonText.text = "Equip";
                GameManager.instance.SaveGame();
            }     
        }
    }

    private void EquipChosenItem(Item item, int index)
    {
        switch (item.type)
        {
            case "Weapon":
                player.EquipItem((Weapon)item);
                EquippedWeaponIndex = index; // Assign index of equipped weapon              
                break;
            case "Armor":
                player.EquipItem((Armor)item);
                EquippedArmorIndex = index; // Assign index of equipped armor 
                break;
            case "Helmet":
                player.EquipItem((Helmet)item);
                EquippedHelmetIndex = index; // Assign index of equipped helmet
                break;
        }
    }

    private void UnequipEquippedItem(Item item)
    {
        switch (item.type)
        {
            case "Weapon":
                GameObject lastESignWeapon = content.transform.GetChild(EquippedWeaponIndex).Find("ESign").gameObject;
                lastESignWeapon.SetActive(false);
                player.UnequipItem((Weapon)item);
                break;

            case "Armor":
                if (EquippedArmorIndex != -1)
                {
                    GameObject lastESignArmor = content.transform.GetChild(EquippedArmorIndex).Find("ESign").gameObject;
                    lastESignArmor.SetActive(false);
                    EquippedArmorIndex = -1;
                    player.UnequipItem((Armor)item);
                }
                break;

            case "Helmet":
                if (EquippedHelmetIndex != -1)
                {
                    GameObject lastESignHelmet = content.transform.GetChild(EquippedHelmetIndex).Find("ESign").gameObject;
                    lastESignHelmet.SetActive(false);
                    EquippedHelmetIndex = -1;
                    player.UnequipItem((Helmet)item);
                }
                break;
        }
    }

    // Drop the item on Drop click and update the player's inventory
    public void OnDropClick(int i)
    {
        if (isEquipped(i))
            Debug.Log("You can't drop an equipped item!");
        else
        {
            DropItem(i);
            RemoveItem(i);
            itemView.SetActive(false);
            UpdateInventory();
        }
    }

    // Drop an item to the screen
    public void DropItem(int i)
    {
        Debug.Log("Dropped item: " + player.Items[i].name);
        GameObject itemDrop = Instantiate(itemDropPrefab);
        itemDrop.GetComponent<ItemManager>().item = player.Items[i];
        itemDrop.GetComponent<SpriteRenderer>().sprite = player.Items[i].itemSprite;
        itemDrop.name = player.Items[i].name;
        itemDrop.transform.position = player.transform.position;
        itemDrop.transform.localScale = player.Items[i].spriteSize;
        ItemManager.instance.DestroyItemDrop(); // Destroy object after a few seconds
    }

    // Remove item from inventory on drop/sell and update other item's indexes
    public void RemoveItem(int index)
    {
        // Move each item one place backwards in the inventory
        for (int i = index; i < LastItem - 1; i++)
        {
            // If equipped, update the equipped item to be one place before
            if (isEquipped(i + 1))
            {
                //Debug.unityLogger.logEnabled = false;
                OnEquipClick(i + 1);
                player.Items[i] = player.Items[i + 1];
                OnEquipClick(i);
               // Debug.unityLogger.logEnabled = true;
            }
            else player.Items[i] = player.Items[i + 1];
        }
        player.Items[LastItem - 1] = null;

        // Show empty on inventory
        Text itemName = content.transform.GetChild(LastItem - 1).Find("ItemName").GetComponent<Text>();
        Image itemImage = content.transform.GetChild(LastItem - 1).Find("ItemImage").GetComponent<Image>();
        itemName.text = "(Empty)";
        itemImage.sprite = null;
        var tempColor = Color.white;
        tempColor.a = 0;
        itemImage.color = tempColor;

        LastItem--;
        GameManager.instance.SaveGame();
    }

    // Is item equipped
    private bool isEquipped(int index)
    {
        return ((index == EquippedArmorIndex) || (index == EquippedHelmetIndex) || (index == EquippedWeaponIndex));
    }

    // Update inventory information
    public void UpdateInventory()
    {
        for (int i = 0; i < inventorySpace; i++)
        {
            Button itemButton = content.transform.GetChild(i).GetComponent<Button>();

            // Remove listeners from buttons with no items
            if (i > LastItem - 1)
            {
                itemButton.onClick.RemoveAllListeners();
            }
            else
            {
                Text itemName = content.transform.GetChild(i).Find("ItemName").GetComponent<Text>();
                Image itemImage = content.transform.GetChild(i).Find("ItemImage").GetComponent<Image>();

                // Set the item object in the inventory view
                itemName.text = player.Items[i].itemName;
                itemImage.sprite = player.Items[i].inverntoryIcon;
                itemImage.color = Color.white;
                int temp = i;
                itemButton.onClick.AddListener(() => OnItemClick(temp));
            }
        }

        // ESign on equipped items
        if (EquippedWeaponIndex != -1)
            content.transform.GetChild(EquippedWeaponIndex).Find("ESign").gameObject.SetActive(true);
        if (EquippedArmorIndex != -1)
            content.transform.GetChild(EquippedArmorIndex).Find("ESign").gameObject.SetActive(true);
        if (EquippedHelmetIndex != -1)
            content.transform.GetChild(EquippedHelmetIndex).Find("ESign").gameObject.SetActive(true);
    }
}

