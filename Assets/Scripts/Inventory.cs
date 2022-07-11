using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Player player;

    // Text fields
    public Text lvlText, xpText, hpText, goldText, upgradeText;

    // Logic
    private int currentCharacter = 0;
    public Image characterSprite;
    public Image weaponSprite;
    public RectTransform xpBar;

    // Character selection
    public void OnArrowClick(bool right)
    {
        if (right)
        {
            currentCharacter++;

            // If went too far
            if (currentCharacter == GameManager.instance.playerSprites.Count)
                currentCharacter = 0;

            OnCharacterSelect();
        }
        else
        {
            currentCharacter--;

            // If went too far
            if (currentCharacter < 0)
                currentCharacter = GameManager.instance.playerSprites.Count-1;

            OnCharacterSelect();
        }
    }
    private void OnCharacterSelect()
    {
        characterSprite.sprite = GameManager.instance.playerSprites[currentCharacter];
    }

    // Weapon upgrade
    public void OnUpgradeClick()
    {
        if (GameManager.instance.tryUpgradeWeapon())
            UpdateInventory();  
    }

    // Update character information
    public void UpdateInventory()
    {
        // Weapon
        weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLvl];
        if (GameManager.instance.weapon.weaponLvl == GameManager.instance.weaponPrices.Count)
            upgradeText.text = "MAX";
        else upgradeText.text = "Upgrade Cost: " + GameManager.instance.weaponPrices[GameManager.instance.weapon.weaponLvl].ToString()+" Gold";

        // Stats
        //lvlText.text = GameManager.instance.GetPlayerLevel().ToString();
        //hpText.text= GameManager.instance.player.hp.ToString();
        //goldText.text = GameManager.instance.gold.ToString();

        lvlText.text = player.GetLevel().ToString();
        hpText.text = player.GetHp().ToString();
        goldText.text = player.GetGold().ToString();

        // Xp bar
        int currentLvl = player.GetLevel();
        if (currentLvl == player.GetXpTable().Count)
        {
            xpText.text = "Total xp: "+ player.GetXp().ToString();
            xpBar.localScale = Vector3.one;
        }
        else
        {
            int prevLvlXp = player.GetXpToLevelUp(currentLvl-1);
            
            int currLvlXp = player.GetXpToLevelUp(currentLvl);

            int diff = currLvlXp - prevLvlXp;
            //wrong, need to substract all prev lvls. CHANAGE XP SYSYEM!
            int currXpInLvl = player.GetXp() - prevLvlXp;

            float completionRatio = (float)currXpInLvl / (float)diff;
            xpBar.localScale = new Vector3(completionRatio, 1, 1);
            xpText.text = currXpInLvl.ToString() + " / " + diff;
        }
    }
}
