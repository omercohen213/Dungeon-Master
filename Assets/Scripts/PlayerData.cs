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
    [SerializeField] private int attributePoints;
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
    public int AttributePoints { get => attributePoints; set => attributePoints = value; }

    // Inventory items
    [SerializeField] private List<int> itemsId; // Holds ids that will be loaded as items when loading data
    public List<int> ItemsId { get => itemsId; set => itemsId = value; }

    [SerializeField] private int [] equippedIndexes; // Holds the equipped items indexes in the inventory in this order [weapon,helmet,armor]
    public int[] EquippedIndexes { get => equippedIndexes; set => equippedIndexes = value; }


    public void Initialize()
    {
        ItemsId = new List<int>();
        equippedIndexes = new int[3];
        for (int i = 0; i < equippedIndexes.Length; i++)
        {
            equippedIndexes[i] = -1;
        }
    }

    // Reset player data to default starting data
    public void ResetPlayerData()
    {       
        // Resources
        gold = 0;
        xp = 0;
        lvl = 1;
        hp = 100;
        maxHp = 100;
        mp = 10;
        maxMp = 10;
        attackPower = 1;
        abilityPower = 1;
        defense = 1;
        magicResist = 1;
        critChance = 0;
        playerName = "New Player";
        attributePoints = 0;

        // Starting items
        Weapon startingWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        ItemsId.Add(startingWeapon.id);
        equippedIndexes[0] = 0;
    }
}
