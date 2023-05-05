using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //References
    private Player player;
    private InventoryManager inventoryManager;
    public FloatingTextManager floatingTextManager;
    public HUD hud;

    private PlayerData playerData = new PlayerData();
    private List<int> xpTable = new List<int>();
    private string saveFilePath;

    private void Awake()
    {
        // to avoid creating two gameManagers
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(floatingTextManager.gameObject);
            Destroy(player.gameObject);
            Destroy(hud.gameObject);
        }
    }

    private void Start()
    {
        player = Player.instance;
        inventoryManager = InventoryManager.instance;
        CreateXpTable();
        player.InitializePlayer();
        playerData.InitializePlayerData();

        saveFilePath = Application.persistentDataPath + "/SaveGame.json";
        //File.Delete(saveFilePath);
        if (File.Exists(saveFilePath))
        {
            Debug.Log("Loading data...");
            LoadGame();
        }
        else
        {
            Debug.Log("Could not find data. Creating a new data file...");
            StartNewGame();
        }
    }
    // Create the data structure that determines how much xp needed for each level to level up for the player
    private void CreateXpTable()
    {
        xpTable.Add(0);
        int xpToLvlUp = 51;
        xpTable.Add(xpToLvlUp); // Lvl 1
        for (int i = 0; i <= 3; i++)
        {
            xpToLvlUp = (int)(xpToLvlUp * 1.8f); // Lvl 2-5
            xpTable.Add(xpToLvlUp);
        }

        int lvl = 5;
        for (int i = 0; i <= 100; i++)
        {
            xpToLvlUp = (int)(0.16666667 * lvl * (lvl - 1) * (1.1 * 2 * (lvl - 1) + 120)); // Lvl 5-100
            xpTable.Add(xpToLvlUp);
            lvl++;
        }
    }

    public int XpToLevelUp(int lvl)
    {
        return xpTable[lvl];
    }

    public void SaveGame()
    {
        // Rescources data
        playerData.PlayerName = player.PlayerName;
        playerData.Lvl = player.Lvl;
        playerData.Xp = player.Xp;
        playerData.Gold = player.Gold;
        playerData.Hp = player.Hp;
        playerData.MaxHp = player.MaxHp;
        playerData.Mp = player.Mp;
        playerData.MaxMp = player.MaxMp;
        playerData.AttackPower = player.AttackPower;
        playerData.AbilityPower = player.AbilityPower;
        playerData.Defense = player.Defense;
        playerData.MagicResist = player.MagicResist;
        playerData.CritChance = player.CritChance;

        // Inventory data
        playerData.LastItem = inventoryManager.LastItem;
        playerData.ItemsId.Clear();
        for (int i = 0; i < inventoryManager.LastItem; i++)
        {
            playerData.ItemsId.Add(player.Items[i].id);
        }

        playerData.EquippedWeaponIndex = inventoryManager.EquippedWeaponIndex;
        playerData.EquippedArmorIndex = inventoryManager.EquippedArmorIndex;
        playerData.EquippedHelmetIndex = inventoryManager.EquippedHelmetIndex;

        // Write to file
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame()
    {
        // Read from file
        string json = File.ReadAllText(saveFilePath);
        playerData = JsonUtility.FromJson<PlayerData>(json);

        // Rescources data
        player.PlayerName = playerData.PlayerName;
        player.Lvl = playerData.Lvl;
        player.Xp = playerData.Xp;
        player.Gold = playerData.Gold;
        player.Hp = playerData.Hp;
        player.MaxHp = playerData.MaxHp;
        player.Mp = playerData.Mp;
        player.MaxMp = playerData.MaxMp;
        player.AttackPower = playerData.AttackPower;
        player.AbilityPower = playerData.AbilityPower;
        player.Defense = playerData.Defense;
        player.MagicResist = playerData.MagicResist;
        player.CritChance = playerData.CritChance;

        // Inventory data
        inventoryManager.LastItem = playerData.LastItem;
        inventoryManager.EquippedWeaponIndex = playerData.EquippedWeaponIndex;
        inventoryManager.EquippedArmorIndex = playerData.EquippedArmorIndex;
        inventoryManager.EquippedHelmetIndex = playerData.EquippedHelmetIndex;

        // Items data
        for (int i = 0; i < playerData.LastItem; i++)
        {
            Item item = FindItemById(playerData.ItemsId[i]);
            player.Items[i] = Resources.Load<Item>("Items/" + item.name);
        }
        Weapon weapon = (Weapon)FindItemById(playerData.ItemsId[playerData.EquippedWeaponIndex]);
        player.EquipItem(weapon);
        if (inventoryManager.EquippedArmorIndex != -1)
        {
            Armor armor = (Armor)FindItemById(playerData.ItemsId[playerData.EquippedArmorIndex]);
            player.EquipItem(armor);
        }
        if (inventoryManager.EquippedHelmetIndex != -1)
        {
            Helmet helmet = (Helmet)FindItemById(playerData.ItemsId[playerData.EquippedHelmetIndex]);
            player.EquipItem(helmet);
        }

        DungeonManager.instance.SpawnPlayer();

    }
    private void StartNewGame()
    {
        playerData.ResetPlayerData();
        string json = JsonUtility.ToJson(playerData);
        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save JSON: " + e.Message);
        }

        LoadGame();
    }

    public Item FindItemById(int id)
    {
        Object[] assets = Resources.LoadAll("Items/", typeof(ScriptableObject));
        foreach (Object asset in assets)
        {
            ScriptableObject scriptableObject = (ScriptableObject)asset;
            SerializedObject serializedObject = new SerializedObject(scriptableObject);
            SerializedProperty property = serializedObject.FindProperty("id");
            if (property != null && property.intValue == id)
            {
                return (Item)scriptableObject;
            }
        }

        return null;
    }

}


