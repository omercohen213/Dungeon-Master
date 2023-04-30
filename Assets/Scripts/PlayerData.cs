using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // Rescources
    private int gold;
    private int xp;
    private int lvl;
    private string playerName;
    private int hp;
    private int maxHp;
    private int mp;
    private int maxMp;
    private int attackPower;
    private int abilityPower;
    private int defense;
    private int magicResist;
    private float critChance;
    private Vector3 position;
    public int Gold { get => gold; set => gold = value; }
    public int Xp { get => xp; set => xp = value; }
    public int Lvl { get => lvl; set => lvl = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Mp { get => mp; set => mp = value; }
    public int MaxMp { get => maxMp; set => maxMp = value; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public int AbilityPower { get => abilityPower; set => abilityPower = value; }
    public int Defense { get => defense; set => defense = value; }
    public int MagicResist { get => magicResist; set => magicResist = value; }
    public float CritChance { get => critChance; set => critChance = value; }
    public Vector3 Position { get => position; set => position = value; }

    // Inventory items
    private List<Item> items;
    public List<Item> Items { get => items; set => items = value; }

    private int lastItem; // Number of items the player owns
    private int equippedWeaponIndex;
    private int equippedArmorIndex;
    private int equippedHelmetIndex;
    public int LastItem { get => lastItem; set => lastItem = value; }
    public int EquippedWeaponIndex { get => equippedWeaponIndex; set => equippedWeaponIndex = value; }
    public int EquippedArmorIndex { get => equippedArmorIndex; set => equippedArmorIndex = value; }
    public int EquippedHelmetIndex { get => equippedHelmetIndex; set => equippedHelmetIndex = value; }

    // Equipped items
    private Weapon weapon;
    private Armor armor;
    private Helmet helmet;
    public Weapon Weapon { get => weapon; set => weapon = value; }
    public Armor Armor { get => armor; set => armor = value; }
    public Helmet Helmet { get => helmet; set => helmet = value; }

    // On log-in
    public void InitializePlayerData()
    {
        Items = new List<Item>();
    }

    // On first run
    public void ResetPlayerData()
    {
        // Resources
        Gold = 0;
        Xp = 0;
        Lvl = 1;
        PlayerName = "New Player";
        Hp = 100;
        MaxHp = 100;
        Mp = 10;
        MaxMp = 10;
        AttackPower = 1;
        AbilityPower = 1;
        Defense = 1;
        MagicResist = 1;
        CritChance = 0;

        // Starting items
        Weapon startingWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        Weapon = startingWeapon;
        Armor = null;
        Helmet = null;
        Items.Add(startingWeapon);

        // Equipped items' index in the items array
        EquippedWeaponIndex = 0;
        EquippedArmorIndex = -1;
        EquippedHelmetIndex = -1;
        LastItem = 1;
    }
}
