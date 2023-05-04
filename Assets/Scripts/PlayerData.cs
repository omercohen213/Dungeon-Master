using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // Rescources
    [SerializeField] private int gold;
    [SerializeField] private int xp;
    [SerializeField] private int lvl;
    [SerializeField] private int hp;
    [SerializeField] private int maxHp;
    [SerializeField] private int mp;
    [SerializeField] private int maxMp;
    [SerializeField] private int attackPower;
    [SerializeField] private int abilityPower;
    [SerializeField] private int defense;
    [SerializeField] private int magicResist;
    [SerializeField] private float critChance;
    [SerializeField] private Vector3 position;
    [SerializeField] private string playerName;
    public int Gold { get => gold; set => gold = value; }
    public int Xp { get => xp; set => xp = value; }
    public int Lvl { get => lvl; set => lvl = value; }
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
    public string PlayerName { get => playerName; set => playerName = value; }

    // Inventory items
    [SerializeField] private List<Item> items;
    public List<Item> Items { get => items; set => items = value; }

    [SerializeField] private int lastItem; // Number of items the player owns
    [SerializeField] private int equippedWeaponIndex;
    [SerializeField] private int equippedArmorIndex;
    [SerializeField] private int equippedHelmetIndex;
    public int LastItem { get => lastItem; set => lastItem = value; }
    public int EquippedWeaponIndex { get => equippedWeaponIndex; set => equippedWeaponIndex = value; }
    public int EquippedArmorIndex { get => equippedArmorIndex; set => equippedArmorIndex = value; }
    public int EquippedHelmetIndex { get => equippedHelmetIndex; set => equippedHelmetIndex = value; }

    // Equipped items
    [SerializeField] private Weapon weapon;
    [SerializeField] private Armor armor;
    [SerializeField] private Helmet helmet;
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
