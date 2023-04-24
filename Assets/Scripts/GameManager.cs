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
        FirstRun();
        //if (File.Exists(@"SaveGame.json"))
        //LoadGame();
        // else FirstRun();
    }

    // Initialize player data when running the game for the first time 
    public void FirstRun()
    {
        playerData.ResetPlayerData();
        gameData.playerDatas.Add(playerData);
        player.InitializePlayer();
        player.LoadPlayer(playerData);

        SaveGame();
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

       /* player.SavePlayer();
        var json = JsonUtility.ToJson(playerData);
        using (StreamWriter streamWriter = new StreamWriter("SaveGame.json"))
        {
            streamWriter.Write(json);
        }*/
    }

    public void LoadGame()
    {
        using (StreamReader streamReader = new StreamReader("SaveGame.json"))
        {
            string json = streamReader.ReadToEnd();
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            var playerData = gameData.playerDatas.Find(x => x.playerName == player.playerName);
            Debug.Log(playerData.xp);
            if (playerData != null)
                player.LoadPlayer(playerData);

            else Debug.Log("Could not load player data of name: " + player.playerName);

        }

        /*player.InitializePlayer();
        playerData.InitializePlayerData();
        using (StreamReader streamReader = new StreamReader("SaveGame.json"))
        {
            string json = streamReader.ReadToEnd();
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log(playerData.lvl);
            player.LoadPlayer(playerData);
        }*/

    }
}




