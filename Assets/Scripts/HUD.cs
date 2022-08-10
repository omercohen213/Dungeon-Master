using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
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
        hpText.text = player.hp + " / " + player.maxHp;
        float hpRatio = (float)player.hp / (float)player.maxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);
    }
    public void onMpChange()
    {
        mpText.text = player.mp + " / " + player.maxMp;
        float mpRatio = (float)player.mp / (float)player.maxMp;
        mpBar.localScale = new Vector3(mpRatio, 1, 1);

    }
    public void onXpChange()
    {
        float xpToLvlUp = GameManager.instance.XpToLevelUp(player.lvl);
        float xp = player.xp;
        float xpPercentage = xp / xpToLvlUp * 100;
        xpText.text = xp + " / " + xpToLvlUp + "(" + xpPercentage.ToString("0.00") + "%)";

        float xpRatio = xp / xpToLvlUp;
        xpBar.localScale = new Vector3(xpRatio, 1, 1);

        lvlText.text = "LVL "+ player.lvl.ToString();
    }
    public void onGoldChange()
    {
        goldText.text = "$" + player.gold.ToString();
    }
    public void onLevelChange()
    {
        lvlText.text = "LVL " + player.lvl.ToString();
    }
    
    private void Start()
    {
        // HP bar
        hpText.text = player.hp + " / " + player.maxHp;
        float hpRatio = (float)player.hp / (float)player.maxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);

        // MP bar
        mpText.text = player.mp + " / " + player.maxMp;
        //float mpRatio = (float)player.GetMp() / (float)player.GetMaxMp();
        //mpBar.localScale = new Vector3(mpRatio, 1, 1);

        // XP bar
        int xpToLvlUp = GameManager.instance.XpToLevelUp(player.lvl);
        int xp = player.xp;
        float xpPercentage = (float)xp / (float)xpToLvlUp * 100; 
        xpText.text = xp + " / " + xpToLvlUp + "(" + xpPercentage.ToString("0.00") + "%)";         
        float xpRatio = (float)xp / (float)xpToLvlUp;
        xpBar.localScale = new Vector3(xpRatio, 1, 1);

        // Lvl
        lvlText.text = "LVL "+ player.lvl.ToString();

        // Gold
        goldText.text = "$" + player.gold.ToString();
    }
}
