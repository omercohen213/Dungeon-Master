using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //References
    [SerializeField] private Player player;
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
        // Clear arrays/lists

        // Rescources data
        playerData.playerName = player.playerName;
        playerData.lvl = player.lvl;
        playerData.xp = player.xp;
        playerData.gold = player.gold;
        playerData.position = player.transform.position; // scene
        playerData.hp = player.hp;
        playerData.maxHp = player.maxHp;
        playerData.mp = player.mp;
        playerData.maxMp = player.maxMp;

        // Inventory data
        playerData.lastItem = player.lastItem;
        for (int i = 0; i < player.lastItem; i++)
        {
            playerData.items.Add(player.items[i]);
        }
        playerData.equippedWeaponIndex = player.equippedWeaponIndex;
        playerData.equippedArmorIndex = player.equippedArmorIndex;
        playerData.equippedHelmetIndex = player.equippedHelmetIndex;

        playerData.weapon = player.weapon; // Starts at index 0 of items array
        if (playerData.equippedHelmetIndex != -1)
            playerData.helmet = player.helmet;
        if (playerData.equippedArmorIndex != -1)
            playerData.armor = player.armor;

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
        player.playerName = playerData.playerName;
        player.lvl = playerData.lvl;
        player.xp = playerData.xp;
        player.gold = playerData.gold;
        player.hp = playerData.hp;
        player.maxHp = playerData.maxHp;
        player.mp = playerData.mp;
        player.maxMp = playerData.maxMp;

        // Inventory data
        player.lastItem = playerData.lastItem;
        player.equippedWeaponIndex = playerData.equippedWeaponIndex;
        player.equippedArmorIndex = playerData.equippedArmorIndex;
        player.equippedHelmetIndex = playerData.equippedHelmetIndex;

        // Items data
        for (int i = 0; i < playerData.lastItem; i++)
        {
            player.items[i] = Resources.Load<Item>("Items/" + playerData.items[i].name);
            Debug.Log(i + " " + player.items[i].name);
        }
        player.EquipItem((Weapon)playerData.items[playerData.equippedWeaponIndex]);
        if (player.equippedArmorIndex != -1)
            player.EquipItem((Armor)playerData.items[playerData.equippedArmorIndex]);
        if (player.equippedHelmetIndex != -1)
            player.EquipItem((Helmet)playerData.items[playerData.equippedHelmetIndex]);

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




