using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Fighter
{
    [SerializeField] private float speed = 2;

    [SerializeField] private HUD hud;

    // Resources
    // ------------------ Change protection level ------------------
    public int gold;
    public int xp;
    public int lvl;
    public int mp;
    public int maxMp;
    public string playerName;
    public int hp;
    public int maxHp;
    public int attackPower; // private set
    public int abilityPower;
    public int defense;
    public int magicResist;
    public float critChance;
    public int abilityPoints;
    public int attributePoints { get; private set; }

    // Damage immunity 
    protected float immuneTime = 1.0f;
    protected float lastImmune;

    // Inventory
    public Item[] items;

    // ------------------Move it to inventoryManager and save data ------------------
    public int lastItem; // Number of items the player has
    public int equippedWeaponIndex;
    public int equippedArmorIndex;
    public int equippedHelmetIndex;

    public Weapon weapon;
    public Armor armor;
    public Helmet helmet;
    public List<Quest> activeQuests;

    // Player Name
    private Camera cam;
    private GameObject playerNameText;
    [SerializeField] public Vector3 offset;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        cam = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerNameText = transform.Find("PlayerNameCanvas/PlayerName").gameObject;
    }

    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position + offset);
        if (playerNameText.transform.position != pos)
            playerNameText.transform.position = pos;
    }

    private void FixedUpdate()
    {
        // arrow keys (returns 1/-1 on key down)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        UpdateMotor(new Vector3(x, y, 0), speed);
    }

    // Recieve damage
    protected override void RecieveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            if (dmg.dmgAmount > 0)
            {
                hp -= dmg.dmgAmount;
                FloatingTextManager.instance.ShowFloatingText(dmg.dmgAmount.ToString(), 30, Color.red, dmg.origin, "Hit", 2.0f);
            }
            else FloatingTextManager.instance.ShowFloatingText("0", 30, Color.red, dmg.origin, "Hit", 2.0f);
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            if (hp <= 0)
            {
                hp = 0;
                Death();
            }
        }
        hud.onHpChange();
    }
    public void GrantGold(int amount)
    {
        gold += amount;
    }

    public void GrantXp(int xpAmount)
    {
        xp += xpAmount;
        if (xp > GameManager.instance.XpToLevelUp(lvl))
        {
            xp = 0;
            OnLevelUp();
        }
        FloatingTextManager.instance.ShowFloatingText("+" + xpAmount + "xp", 12, Color.magenta, transform.position, "GetResource", 1.5f);
        GameManager.instance.SaveGame();
        hud.onXpChange();
    }

    public void OnLevelUp()
    {
        // To not instantly show lvl up text on load 
        if (Time.time > 1)
            FloatingTextManager.instance.ShowFloatingText("Level Up!", 30, Color.magenta, transform.position, "GetResource", 1.5f);
        lvl++;
        hp = maxHp;
        abilityPoints++;
        attributePoints++;
        hud.onHpChange();
        hud.onLevelChange();
    } 

    public Weapon GetWeapon()
    {
        return weapon;
    }
    public Armor GetArmor()
    {
        return armor;
    }
    public Helmet GetHelmet()
    {
        return helmet;
    }

    public void EquipItem(Item item)
    {
        switch (item.type)
        {
            case "Weapon":
                Weapon weapon = (Weapon)item;
                this.weapon = weapon;
                transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                attackPower += weapon.attackPower;
                break;
            case "Armor":
                Armor armor = (Armor)item;
                this.armor = armor;
                transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                defense += armor.defense;
                break;
            case "Helmet":
                Helmet helmet = (Helmet)item;
                this.helmet = helmet;
                transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = item.itemSprite; 
                defense += helmet.defense;
                break;
        }
    }

    public void UnequipItem(Item item)
    {
        switch (item.type)
        {
            case "Weapon":
                attackPower -= ((Weapon)item).attackPower;
                break;
            case "Armor":
                armor = null;
                transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = null;
                defense -= ((Armor)item).defense;
                break;
            case "Helmet":
                helmet = null;
                transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                defense -= ((Helmet)item).defense;
                break;
        }       
    }

    public void InitializePlayer()
    {
        items = new Item[15];
        
    }
}
