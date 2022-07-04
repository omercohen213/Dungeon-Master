using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        // to avoid creating two gameManagers
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }   

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject); 
    }
    //Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    //References
    public Player player;

    //public weapon

    //logic
    public int gold;
    public int xp;

    //floating text
    public FloatingTextManager floatingTextManager;

    public void ShowText(string text, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(text, fontSize, color, position, motion, duration );
    }

    /* preferedSkin
     * gold 
     * xp
     * weaponLevel */
    public void SaveState()
    {
        ;

        string s = "";
        s += "0" + "|";
        s+= gold.ToString() + "|";
        s+= xp.ToString() + "|";
        s += "0";

        PlayerPrefs.SetString("SaveState", s);

        Debug.Log("SaveState");
    }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        //change player skin
        gold = int.Parse(data[1]);
        xp = int.Parse(data[2]);
        // change weapon lvl

        Debug.Log(data[1] +" "+ data[2]);
        Debug.Log("LoadState");
    }
}




