using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

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
        //File.Delete(@"SaveGame.json");

        if (File.Exists(@"SaveGame.json"))
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
        playerData.Position = player.transform.position;
        playerData.Hp = player.Hp;
        playerData.MaxHp = player.MaxHp;
        playerData.Mp = player.Mp;
        playerData.MaxMp = player.MaxMp;
        playerData.AttackPower = player.AttackPower;
        playerData.AbilityPower = player.AbilityPower;
        playerData.Defense = player.Defense ;
        playerData.MagicResist = player.MagicResist;
        playerData.CritChance = player.CritChance;

        // Inventory data
        playerData.LastItem = inventoryManager.LastItem;
        playerData.Items.Clear();
        for (int i = 0; i < inventoryManager.LastItem; i++)
        {
            playerData.Items.Add(player.Items[i]);
        }
        playerData.EquippedWeaponIndex = inventoryManager.EquippedWeaponIndex;
        playerData.EquippedArmorIndex = inventoryManager.EquippedArmorIndex;
        playerData.EquippedHelmetIndex = inventoryManager.EquippedHelmetIndex;

        playerData.Weapon = player.weapon;
        if (playerData.EquippedHelmetIndex != -1)
            playerData.Helmet = player.helmet;
        if (playerData.EquippedArmorIndex != -1)
            playerData.Armor = player.armor;

        // Write to file
        string json = JsonUtility.ToJson(playerData);
        using (StreamWriter streamWriter = new StreamWriter("SaveGame.json"))
        {
            streamWriter.Write(json);
        }
    }

    public void LoadGame()
    {
        // Read from file
        using (StreamReader streamReader = new StreamReader("SaveGame.json"))
        {
            string json = streamReader.ReadToEnd();
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }

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
            player.Items[i] = Resources.Load<Item>("Items/" + playerData.Items[i].name);
        }

        player.EquipItem((Weapon)playerData.Items[playerData.EquippedWeaponIndex]);
        if (inventoryManager.EquippedArmorIndex != -1)
            player.EquipItem((Armor)playerData.Items[playerData.EquippedArmorIndex]);
        if (inventoryManager.EquippedHelmetIndex != -1)
            player.EquipItem((Helmet)playerData.Items[playerData.EquippedHelmetIndex]);

        // Spawn point
        RectTransform portalRectTransform = GameObject.Find("SpawnPoint").GetComponent<RectTransform>();
        Transform portal = GameObject.Find("SpawnPoint").transform;
        float portalWidth = portalRectTransform.rect.width * 0.16f;
        float portalHeight = portalRectTransform.rect.height * 0.16f;
        player.transform.position = portal.position + new Vector3(portalWidth, -portalHeight / 3, 0);
    }

    private void StartNewGame()
    {
        playerData.ResetPlayerData();
        // Write to file
        string json = JsonUtility.ToJson(playerData);
        using (StreamWriter streamWriter = new StreamWriter("SaveGame.json"))
        {
            streamWriter.Write(json);
        }
        LoadGame();
    }
}




