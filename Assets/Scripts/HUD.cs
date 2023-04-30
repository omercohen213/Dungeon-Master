using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    //References
    [SerializeField] private Player player;
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private Text hpText;
    [SerializeField] private RectTransform mpBar;
    [SerializeField] private Text mpText;
    [SerializeField] private RectTransform xpBar;
    [SerializeField] private Text xpText;
    [SerializeField] private Text lvlText;
    [SerializeField] private Text goldText;

    public void onHpChange()
    {
        hpText.text = player.Hp + " / " + player.MaxHp;
        float hpRatio = (float)player.Hp / (float)player.MaxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);
    }
    public void onMpChange()
    {
        mpText.text = player.Mp + " / " + player.MaxMp;
        float mpRatio = (float)player.Mp / (float)player.MaxMp;
        mpBar.localScale = new Vector3(mpRatio, 1, 1);

    }
    public void onXpChange()
    {
        float xpToLvlUp = GameManager.instance.XpToLevelUp(player.Lvl);
        float xp = player.Xp;
        float xpPercentage = xp / xpToLvlUp * 100;
        xpText.text = xp + " / " + xpToLvlUp + "(" + xpPercentage.ToString("0.00") + "%)";

        float xpRatio = xp / xpToLvlUp;
        xpBar.localScale = new Vector3(xpRatio, 1, 1);

        lvlText.text = "LVL "+ player.Lvl.ToString();
    }
    public void onGoldChange()
    {
        goldText.text = "$" + player.Gold.ToString();
    }
    public void onLevelChange()
    {
        lvlText.text = "LVL " + player.Lvl.ToString();
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // HP bar
        hpText.text = player.Hp + " / " + player.MaxHp;
        float hpRatio = (float)player.Hp / (float)player.MaxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);

        // MP bar
        mpText.text = player.Mp + " / " + player.MaxMp;
        //float mpRatio = (float)player.GetMp() / (float)player.GetMaxMp();
        //mpBar.localScale = new Vector3(mpRatio, 1, 1);

        // XP bar
        int xpToLvlUp = GameManager.instance.XpToLevelUp(player.Lvl);
        int xp = player.Xp;
        float xpPercentage = (float)xp / (float)xpToLvlUp * 100; 
        xpText.text = xp + " / " + xpToLvlUp + "(" + xpPercentage.ToString("0.00") + "%)";         
        float xpRatio = (float)xp / (float)xpToLvlUp;
        xpBar.localScale = new Vector3(xpRatio, 1, 1);

        // Lvl
        lvlText.text = "LVL "+ player.Lvl.ToString();

        // Gold
        goldText.text = "$" + player.Gold.ToString();
    }
}
