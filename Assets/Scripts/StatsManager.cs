using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;
    private Player player;

    [SerializeField] private Text statsText;
    [SerializeField] private Text attributePointsText;
    [SerializeField] private GameObject attributeButtonPrefab;
    [SerializeField] private Transform AttributeButtonsParent;

    [SerializeField] private GameObject weaponObject;
    [SerializeField] private GameObject helmetObject;
    [SerializeField] private GameObject armorObject;

    private readonly int attributeButtonsNum = 7;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        player = Player.instance;
        for (int i = 0; i < attributeButtonsNum; i++)
        {
            GameObject attributeButton = Instantiate(attributeButtonPrefab, AttributeButtonsParent);
            attributeButton.name = "AttributeButton " + i;
            attributeButton.GetComponent<Button>().onClick.AddListener(() => OnAttributeButtonClick());
            attributeButton.SetActive(false);
        }
        UpdatePlayerStats();
        gameObject.SetActive(false);
    }

    // Add points to selected stat
    public void OnAttributeButtonClick()
    {
        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            // Max Hp
            case "AttributeButton 0":
                player.MaxHp += 10;
                player.Hp += 10;
                HUD.instance.onHpChange();
                break;
            // Max Mp
            case "AttributeButton 1":
                player.MaxMp += 10;
                player.Mp += 10;
                HUD.instance.onMpChange();
                break;
            // Attack power
            case "AttributeButton 2":
                player.AttackPower += 1;
                break;
            // Ability power
            case "AttributeButton 3":
                player.AbilityPower += 1;
                break;
            // Defense
            case "AttributeButton 4":
                player.Defense += 1;
                break;
            // Magic resist
            case "AttributeButton 5":
                player.MagicResist += 1;
                break;
            // Crit chance
            case "AttributeButton 6":
                player.CritChance += 0.33f;
                break;
        }
        UpdatePlayerStats();
        player.AttributePoints--;
        GameManager.instance.SaveGame();
    }

    // Update player stats object
    private void UpdatePlayerStats()
    {
        statsText.text = "Max HP: " + player.MaxHp + "\n" +
            "Max MP: " + player.MaxMp + "\n" +
            "Attack Power: " + player.GetTotalAttackPower() + "\n" +
            "Ability Power: " + player.AbilityPower + "\n" +
            "Defense: " + player.GetTotalDefense() + "\n" +
            "Magic Resist: " + player.MagicResist + "\n" +
            "Critical Hit Chance: " + player.CritChance + "%\n";

        bool activateButtons;
        if (player.AttributePoints > 0)
        {
            activateButtons = true;
            attributePointsText.text = "Attribute Points: " + player.AttributePoints;
        }
        else
        {
            activateButtons = false;
            attributePointsText.text = "";
        }

        for (int i = 0; i < attributeButtonsNum; i++)
        {
            AttributeButtonsParent.transform.GetChild(i).gameObject.SetActive(activateButtons);
        }
    }

    // Load sprites of character and the items in character preview
    private void LoadCharachterSprite()
    {
        Image weaponImage = weaponObject.GetComponent<Image>();
        Image armorImage = armorObject.GetComponent<Image>();
        Image helmetImage = helmetObject.GetComponent<Image>();

        weaponImage.sprite = player.Weapon.itemSprite;

        if (player.Helmet != null)
        {
            helmetImage.sprite = player.Helmet.itemSprite;
            var tempColor = helmetImage.color;
            tempColor.a = 255f;
            helmetImage.color = tempColor;
        }
        else
        {
            var tempColor = helmetImage.color;
            tempColor.a = 0;
            helmetImage.color = tempColor;
        }
        if (player.Armor != null)
        {
            armorImage.sprite = player.Armor.itemSprite;
            var tempColor = armorImage.color;
            tempColor.a = 255f;
            armorImage.color = tempColor;
        }
        else
        {
            var tempColor = armorImage.color;
            tempColor.a = 0;
            armorImage.color = tempColor;
        }
    }

    // Opening/closing stats
    public void OnPlayerStatsClick()
    {
        gameObject.SetActive(true);
        LoadCharachterSprite();
        UpdatePlayerStats();

        /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("Inventory_closed"))
        {
            UpdateInventory();
            animator.SetTrigger("open");
            itemView.SetActive(false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Inventory_opened"))
            animator.SetTrigger("close");*/
    }
}
