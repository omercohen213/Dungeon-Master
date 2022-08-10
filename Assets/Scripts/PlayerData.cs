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
    public Item[] items;
    public int lastItem; // Number of items the player has
    public int equippedWeaponIndex;
    public int equippedArmorIndex;
    public int equippedHelmetIndex;

    // Equipped items
    public Weapon weapon;
    public Armor armor;
    public Helmet helmet;

    

    /*   // Starting items on Registration
       public Weapon startingWeapon;
       public Armor startingArmor;
       public Helmet startingHelmet;*/

    // On log-in
    public void InitializePlayerData()
    {
        playerName = "IchBinSpite";     
        //items = new List<Item>();
        items = new Item[15];

        // check if first time registered
        Weapon firstWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        items[0] = firstWeapon;
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
        items = new Item[15];
        Weapon firstWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        items[0] = firstWeapon;
        lastItem = 1;
        

        //items = new List<Item>() { startingWeapon , startingArmor, startingHelmet}; // (need to initialize items)
    }

    
 /*   public void Init(Player player)
    {
        this.player = player;
    }*/
}
