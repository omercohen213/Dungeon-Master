using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // GameManager instance
    public static GameManager instance;

    //Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;

    //References
    [SerializeField] private Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public HUD hud;
   


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
        
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    // Show floating text
    public void ShowText(string text, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(text, fontSize, color, position, motion, duration);
    }

    // Upgrade weapon

   /* public int GetPlayerLevel()
    {
        int r = 0;
        int add = 0;
        while (xp >= add)
        {
            add += xpTable[r];
            r++;
            if (r == xpTable.Count)
                return r;

        }
        return r;
    }

    public int GetXpToLevelUp(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }

        return xp;
    }

    public void GrantXp(int xp)
    {
        int currLevel = GetPlayerLevel();
        this.xp += xp;
        if (currLevel < GetPlayerLevel())
            OnLevelUp();
    }

    public void OnLevelUp()
    {
        // To not instantly show text on load 
        if (Time.time > 1)
            ShowText("Level Up!", 25, Color.magenta, player.transform.position, Vector3.up * 25, 2.0f);
        player.OnLevelUp();
    }*/

    // Save game state when loading a new scene 
    public void SaveState()
    {
        string s = "";
        s += player.GetGold().ToString() + "|";
        s += player.GetXp().ToString() + "|";
        s += weapon.weaponLvl.ToString();

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
        
        // Change weapon
        weapon.SetWeaponLvl(int.Parse(data[2]));

        Debug.Log(data[0] + " " + data[1] + " " + data[2]);    

    }
}




