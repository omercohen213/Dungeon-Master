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
        statsText.text = "Max HP: " + player.maxHp + "\n" +
            "Max MP: " + player.maxMp + "\n" +
            "Attack Power: " + player.attackPower + "\n" +
            "Ability Power: " + player.abilityPower + "\n" +
            "Armor: " + player.defense + "\n" +
            "Magic Resist: " + player.magicResist + "\n" +
            "Critical Hit Chance: " + player.critChance + "%\n";

        //Debug.Log(player.GetWeapon().itemName);
        Image weaponImage = weapon.GetComponent<Image>();
        weaponImage.sprite = player.weapon.GetComponent<SpriteRenderer>().sprite;
    }
}
