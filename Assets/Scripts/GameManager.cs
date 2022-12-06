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

    private GameData gameData = new GameData();
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
        else if (instance != null)
        {
            Destroy(floatingTextManager.gameObject);
            Destroy(player.gameObject);
            Destroy(hud.gameObject);
        }
    }

    private void Start()
    {
        CreateXpTable();
        Register();
        //LogIn();     
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
}




