using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    //public Player player;

    // Rescources
    public int gold;
    public int xp;
    public int lvl;
    public string playerName;
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public Vector3 position;

    // Inventory items
    //public List<Item> items;
    //public Item[] items;

    public List<string> itemNames;
    public string[] types;
    public int[] requiredLvls;
    public Sprite[] inverntoryIcons;
    public Sprite[] itemSprites;
    public Vector3[] spriteSizes;
    public int[] prices;
    public float[] dropRates; // Percentage; In range 0-100

    public int lastItem; // Number of items the player has
    public int equippedWeaponIndex;
    public int equippedArmorIndex;
    public int equippedHelmetIndex;

    // Equipped items
    public Weapon weapon;
    public Armor armor;
    public Helmet helmet;

    // Starting items on Registration
    // public Weapon startingWeapon;

    // On log-in
    public void InitializePlayerData()
    {
        playerName = "IchBinSpite";
        itemNames = new List<string>();
        types = new string[15];
        requiredLvls = new int[15];
        inverntoryIcons = new Sprite[15];
        itemSprites = new Sprite[15];
        spriteSizes = new Vector3[15];
        prices = new int[15];
        dropRates = new float[15];

        // check if first time registered

        Weapon firstWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        itemNames.Add(firstWeapon.itemName);
        types[0] = firstWeapon.itemName;
        requiredLvls[0] = firstWeapon.requiredLvl;
        inverntoryIcons[0] = firstWeapon.inverntoryIcon;
        itemSprites[0] = firstWeapon.itemSprite;
        spriteSizes[0] = firstWeapon.spriteSize;
        prices[0] = firstWeapon.price;
        dropRates[0] = firstWeapon.dropRate;
    }

    // On registration
    public void ResetPlayerData()
    {
        gold = 0;
        xp = 0;
        lvl = 1;
        playerName = "IchBinSpite";
        hp = 100;
        maxHp = 100;
        mp = 10;
        maxMp = 10;
        lastItem = 0;
        equippedWeaponIndex = 0; // Equipped items' index in the items array
        equippedArmorIndex = -1;
        equippedHelmetIndex = -1;
        //items = new List<Item>();

        itemNames = new List<string>();
        types = new string[15];
        requiredLvls = new int[15];
        inverntoryIcons = new Sprite[15];
        itemSprites = new Sprite[15];
        spriteSizes = new Vector3[15];
        prices = new int[15];
        dropRates = new float[15];
        lastItem = 1;
    }
}
