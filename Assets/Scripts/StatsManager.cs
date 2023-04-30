using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Text statsText;

    [SerializeField] private Text abilityPoints;

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject helmet;
    [SerializeField] private GameObject armor;

    void Start()
    {
        statsText.text = "Max HP: " + player.MaxHp + "\n" +
            "Max MP: " + player.MaxMp + "\n" +
            "Attack Power: " + player.AttackPower + "\n" +
            "Ability Power: " + player.AbilityPower + "\n" +
            "Armor: " + player.Defense + "\n" +
            "Magic Resist: " + player.MagicResist + "\n" +
            "Critical Hit Chance: " + player.CritChance + "%\n";

        //Debug.Log(player.GetWeapon().itemName);
        Image weaponImage = weapon.GetComponent<Image>();
        weaponImage.sprite = player.weapon.GetComponent<SpriteRenderer>().sprite;
    }
}
