using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //References
    [SerializeField] private Player player;
    public FloatingTextManager floatingTextManager;
    public HUD hud;

    private GameData gameData = new GameData();
    private PlayerData playerData = new PlayerData();
    private List<int> xpTable = new List<int>();

    private void Awake()
    {
        // to avoid creating two gameManagers
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(floatingTextManager.gameObject);
            Destroy(player.gameObject);
            Destroy(hud.gameObject);
        }


        //SceneManager.sceneLoaded += LoadState;
        //SceneManager.sceneLoaded += onSceneLoaded;
    }

    private void Start()
    {
        CreateXpTable();
        //Register();
        LogIn();     
    }

    // Initialize player data on log-in
    public void LogIn()
    {
        // Initialize properties   
        playerData.InitializePlayerData();
        player.InitializePlayer();

        // Then assign it to the player
        LoadGame();
    }

    // Initialize player data on first time playing the game
    public void Register()
    {
        playerData.ResetPlayerData();
        gameData.playerDatas.Add(playerData);
        player.InitializePlayer();
        player.LoadPlayer(playerData);
        
        SaveGame();
    }

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
        // To avoid saving multiple times for the same player
        var playerData = gameData.playerDatas.Find(x => x.playerName == player.playerName);
        if (playerData != null)
            gameData.playerDatas.Remove(playerData);

        player.SavePlayer();
        gameData.playerDatas.Add(player.playerData);

        var json = JsonUtility.ToJson(gameData);
        using (StreamWriter streamWriter = new StreamWriter("SaveGame.json"))
        {
            streamWriter.Write(json);
        }
    }

    public void LoadGame()
    {
        using (StreamReader streamReader = new StreamReader("SaveGame.json"))
        {
            string json = streamReader.ReadToEnd();
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            var playerData = gameData.playerDatas.Find(x => x.playerName == player.playerName);
            if (playerData != null)            
                player.LoadPlayer(playerData);
            
            else Debug.Log("Could not load player data of name: " + player.playerName);

        }
    }

    
    // Save game state when loading a new scene 
    /*    public void SaveState()
        {
            string s = "";
            s += player.GetGold().ToString() + "|";
            s += player.GetXp().ToString() + "|";
            s += player.GetWeapon().itemName;

            PlayerPrefs.SetString("SaveState", s);
        }

        public void onSceneLoaded(Scene s, LoadSceneMode mode)
        {
            // Spawn point
            RectTransform portalRectTransform = GameObject.Find("SpawnPoint").GetComponent<RectTransform>();
            Transform portal = GameObject.Find("SpawnPoint").transform;
            float portalWidth = portalRectTransform.rect.width * 0.16f;
            float portalHeight = portalRectTransform.rect.height * 0.16f;

            player.transform.position = portal.position + new Vector3 (portalWidth, -portalHeight/3, 0);
        }


        // Load game state when loading a new scene 
        public void LoadState(Scene s, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= LoadState;

            if (!PlayerPrefs.HasKey("SaveState"))
                return;

            string[] data = PlayerPrefs.GetString("SaveState").Split('|');

            player.SetGold(int.Parse(data[0]));

            // Xp
            player.SetXp(int.Parse(data[1]));
            if (player.GetLevel() != 1)
                player.SetLevel(player.GetLevel());


            Debug.Log(data[0] + " " + data[1] + " " + data[2]);    

        }*/
}




