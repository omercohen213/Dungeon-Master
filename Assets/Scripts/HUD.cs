using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //References
    [SerializeField] private Player player;

    public Inventory inventory;
    public GameObject inventoryExitButton;
    public Animator InventoryAnim;

    public RectTransform hpBar;
    public Text hpText;
    public RectTransform mpBar;
    public Text mpText;
    public RectTransform xpBar;
    public Text xpText;
    public Text lvlText;
    public Text goldText;

    // Healthbar
    public void onHpChange()
    {
        hpText.text = player.GetHp() + " / " + player.GetMaxHp();
        float hpRatio = (float)player.GetHp() / (float)player.GetMaxHp();
        hpBar.localScale = new Vector3(hpRatio, 1, 1);
    }
    public void onMpChange()
    {
        mpText.text = player.GetMp() + " / " + player.GetMaxMp();
        float mpRatio = (float)player.GetMp() / (float)player.GetMaxMp();
        mpBar.localScale = new Vector3(mpRatio, 1, 1);

    }
    public void onXpChange()
    {
        int currentLvl = player.GetLevel();
        int prevLvlXp = player.GetXpToLevelUp(currentLvl - 1);
        int currLvlXp = player.GetXpToLevelUp(currentLvl);
        int diff = currLvlXp - prevLvlXp;
        int currXpInLvl = player.GetXp() - prevLvlXp;  // need to substract all prev lvls. CHANAGE XP SYSYEM!

        xpText.text = currXpInLvl.ToString() + " / " + diff;
        float xpRatio = (float)currXpInLvl / (float)diff;
        xpBar.localScale = new Vector3(xpRatio, 1, 1);
    }

    public void onGoldChange()
    {
        goldText.text = "$" + player.GetGold().ToString();
    }

    public void onLevelChange()
    {
        lvlText.text = player.GetLevel().ToString();
    }
    // Show inventory on button click
    public void OnInventoryClick()
    {
        if (InventoryAnim.GetCurrentAnimatorStateInfo(0).IsName("Inventory_showing"))
        {
            InventoryAnim.SetTrigger("hide");
            inventoryExitButton.SetActive(false);
        }

        if (InventoryAnim.GetCurrentAnimatorStateInfo(0).IsName("Inventory_hidden"))
        {
            inventory.UpdateInventory();
            InventoryAnim.SetTrigger("show");
            inventoryExitButton.SetActive(true);
        }
    }

    private void Start()
    {
        // Set up hud bars
        hpText.text = player.GetHp() + " / " + player.GetMaxHp();
        float hpRatio = (float)player.GetHp() / (float)player.GetMaxHp();
        hpBar.localScale = new Vector3(hpRatio, 1, 1);

        mpText.text = player.GetMp() + " / " + player.GetMaxMp();
        //float mpRatio = (float)player.GetMp() / (float)player.GetMaxMp();
        //mpBar.localScale = new Vector3(mpRatio, 1, 1);
        
        int currentLvl = player.GetLevel();
        int prevLvlXp = player.GetXpToLevelUp(currentLvl - 1);
        int currLvlXp = player.GetXpToLevelUp(currentLvl);
        int diff = currLvlXp - prevLvlXp;    
        int currXpInLvl = player.GetXp() - prevLvlXp; // need to substract all prev lvls. CHANAGE XP SYSYEM!

        xpText.text = currXpInLvl.ToString() + " / " + diff;
        float xpRatio = (float)currXpInLvl / (float)diff;
        xpBar.localScale = new Vector3(xpRatio, 1, 1);

        goldText.text = "$" + player.GetGold().ToString();
    }
}
