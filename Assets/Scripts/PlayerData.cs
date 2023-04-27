using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // Rescources
    public int gold;
    public int xp;
    public int lvl;
    public string playerName;
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public int attackPower;
    public int abilityPower;
    public int defense;
    public int magicResist;
    public float critChance;
    public Vector3 position;

    // Inventory items
    public List<Item> items;
    public Dictionary<int, int> itemIndexMap;

    public int lastItem; // Number of items the player owns
    public int equippedWeaponIndex;
    public int equippedArmorIndex;
    public int equippedHelmetIndex;

    // Equipped items
    public Weapon weapon;
    public Armor armor;
    public Helmet helmet;

    // On log-in
    public void InitializePlayerData()
    {
        items = new List<Item>();
        itemIndexMap = new Dictionary<int, int>();     
    }

    // On first run
    public void ResetPlayerData()
    {
        // Resources
        gold = 0;
        xp = 0;
        lvl = 1;
        playerName = "New Player";
        hp = 100;
        maxHp = 100;
        mp = 10;
        maxMp = 10;
        attackPower = 1;
        abilityPower = 1;
        defense = 1;
        magicResist = 1;
        critChance = 0;
       
        // Starting items
        Weapon startingWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        weapon = startingWeapon;
        armor = null;
        helmet = null;
        items.Add(startingWeapon);
        itemIndexMap.Add(startingWeapon.GetInstanceID(), 0);       

        // Equipped items' index in the items array
        equippedWeaponIndex = 0; 
        equippedArmorIndex = -1;
        equippedHelmetIndex = -1;
        lastItem = 1;
    }
}
