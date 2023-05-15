using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private Player player;
    private GameManager gameManager;
    private ItemDropManager itemDropManager;

    private int inventorySpace = 15;
    public int InventorySpace { get => inventorySpace; set => inventorySpace = value; }

    private List<Item> items;
    public List<Item> Items { get => items; set => items = value; }

    [SerializeField] private int[] equippedIndexes; // Holds the equipped items indexes in the inventory
    public int[] EquippedIndexes { get => equippedIndexes; set => equippedIndexes = value; }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        player = Player.instance;
        gameManager = GameManager.instance;
        itemDropManager = ItemDropManager.instance;
    }

    // Equip item without save for GameManager.Load()
    public void EquipItemNoSave(Item item, int index)
    {
        switch (item.type)
        {
            case "Weapon":
                EquipWeapon(item, index); break;
            case "Helmet":
                EquipHelmet(item, index); break;
            case "Armor":
                EquipArmor(item, index); break;
        }
    }

    // Checks item type and equips the corresponding one in given index
    public void EquipItem(Item item, int index)
    {
        switch (item.type)
        {
            case "Weapon":
                EquipWeapon(item, index); break;
            case "Helmet":
                EquipHelmet(item, index); break;
            case "Armor":
                EquipArmor(item, index); break;
        }
        gameManager.SaveGame();
    }

    public void EquipWeapon(Item item, int index)
    {
        if (item.type != "Weapon")
        {
            Debug.Log("Not a weapon");
            return;
        }

        player.transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        player.Weapon = item as Weapon;
        equippedIndexes[0] = index;
    }
    public void EquipHelmet(Item item, int index)
    {
        if (item.type != "Helmet")
        {
            Debug.Log("Not a helmet");
            return;
        }
        player.transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        player.Helmet = item as Helmet;
        equippedIndexes[1] = index;
    }

    public void EquipArmor(Item item, int index)
    {
        if (item.type != "Armor")
        {
            Debug.Log("Not an armor");
            return;
        }
        player.transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        player.Armor = item as Armor;
        equippedIndexes[2] = index;
    }

    // Checks item type and unequips the corresponding
    public void UnequipItem(Item item)
    {
        switch (item.type)
        {
            case "Weapon":
                Debug.Log("You can't unequip your weapon!"); break;
            case "Armor":
                UnequipArmor(item); break;
            case "Helmet":
                UnequipHelmet(item); break;
        }
        gameManager.SaveGame();
    }
    public void UnequipHelmet(Item item)
    {
        player.Helmet = null;
        player.transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = null;
        equippedIndexes[1] = -1;
    }
    public void UnequipArmor(Item item)
    {
        player.Armor = null;
        player.transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = null;
        equippedIndexes[2] = -1;
    }
    public void DropItem(Item item, int index)
    {
        itemDropManager.CreateItemDrop(item, player.transform.position);
        items.RemoveAt(index);
        UpdateEquippedIndexes(index);
        gameManager.SaveGame();
    }

    private void UpdateEquippedIndexes(int index)
    {
        for (int i = 0; i < equippedIndexes.Length; i++)
        {
            if (equippedIndexes[i] != -1 && index < equippedIndexes[i])
                equippedIndexes[i]--;
        }
    }

    public bool isEquipped(int index)
    {
        for (int i = 0; i < equippedIndexes.Length; i++)
        {
            if (equippedIndexes[i] == index)
                return true;
        }
        return false;
    }

    // Add item to player items
    public void AddItem(Item item)
    {
        items.Add(item);
        GameManager.instance.SaveGame();
    }

    public bool isFull()
    {
        return (items.Count >= inventorySpace);
    }

    public void Initialize()
    {
        items = new List<Item>();
        equippedIndexes = new int[3];
    }
}
