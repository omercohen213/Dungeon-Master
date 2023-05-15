using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //References
    [SerializeField] private Player player;
    [SerializeField] private Inventory inventory;
    [SerializeField] private FloatingTextManager floatingTextManager;
    [SerializeField] private HUD hud;

    private PlayerData playerData;
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
        playerData = new PlayerData();
        CreateXpTable();
        player.Initialize();
        inventory.Initialize();      
        playerData.Initialize();     
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
        playerData.AttributePoints = player.AttributePoints;

        // Items data
        playerData.ItemsId.Clear();
        for (int i = 0; i < inventory.Items.Count; i++)
        {
            Item item = inventory.Items[i];
            playerData.ItemsId.Add(item.id);
        }
        for (int i = 0; i < playerData.EquippedIndexes.Length; i++)
        {
            playerData.EquippedIndexes[i] = inventory.EquippedIndexes[i];
        }
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
        player.AttributePoints = playerData.AttributePoints;
        player.transform.Find("PlayerNameCanvas/PlayerName").GetComponent<Text>().text = player.PlayerName;

        // Items data
        for (int i = 0; i < playerData.ItemsId.Count; i++)
        {
            Item item = FindItemById(playerData.ItemsId[i]);
            inventory.Items.Add(Resources.Load<Item>("Items/" + item.name));
        }
      
        for (int i = 0; i < playerData.EquippedIndexes.Length; i++)
        {
            inventory.EquippedIndexes[i] = playerData.EquippedIndexes[i];
            int equippedIndex = playerData.EquippedIndexes[i];
            if (equippedIndex != -1) { 
                inventory.EquipItemNoSave(inventory.Items[equippedIndex], equippedIndex);
            }
        }
        DungeonManager.instance.SpawnPlayer();
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

        Debug.Log("item null");
        return null;
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


}


