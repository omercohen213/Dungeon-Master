using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Fighter , IDamageable
{
    public static Player instance;

    private float speed = 2;
    
    // Resources
    private int lvl;
    private int xp;
    private int gold; 
    private int hp;
    private int maxHp;
    private int mp;
    private int maxMp;
    private int attackPower;
    private int abilityPower;
    private int defense;
    private int magicResist;
    private float critChance;
    private int abilityPoints;
    private int attributePoints;
    private string playerName;
    public int Lvl { get => lvl; set => lvl = value; }
    public int Xp { get => xp; set => xp = value; }
    public int Gold { get => gold; set => gold = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Mp { get => mp; set => mp = value; }
    public int MaxMp { get => maxMp; set => maxMp = value; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public int AbilityPower { get => abilityPower; set => abilityPower = value; }
    public int Defense { get => defense; set => defense = value; }
    public int MagicResist { get => magicResist; set => magicResist = value; }
    public float CritChance { get => critChance; set => critChance = value; }
    public int AbilityPoints { get => abilityPoints; set => abilityPoints = value; }
    public int AttributePoints { get => attributePoints; set => attributePoints = value; }
    public string PlayerName { get => playerName; set => playerName = value; }


    // Damage immunity 
    protected float immuneTime = 1.0f;
    protected float lastImmune;

    // Inventory
    private Item[] items;
    public Item[] Items { get => items; set => items = value; }

    private Weapon weapon;
    private Armor armor;
    private Helmet helmet;
    public Weapon Weapon { get => weapon; set => weapon = value; }
    public Armor Armor { get => armor; set => armor = value; }
    public Helmet Helmet { get => helmet; set => helmet = value; }

    public List<Quest> activeQuests;

    // Player Name
    private Camera cam;
    private GameObject playerNameGo;
    [SerializeField] public Vector3 offset;

    private HUD hud;
    private GameManager gameManager;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
        hud = HUD.instance;
        gameManager = GameManager.instance;
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerNameGo = transform.Find("PlayerNameCanvas/PlayerName").gameObject;
        playerNameGo.GetComponent<Text>().text = playerName;
    }

    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position + offset);
        if (playerNameGo.transform.position != pos)
            playerNameGo.transform.position = pos;
    }

    private void FixedUpdate()
    {
        // arrow keys (returns 1/-1 on key down)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        UpdateMotor(new Vector3(x, y, 0), speed);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        cam = Camera.main;
    }
    
    public void GrantGold(int amount)
    {
        Gold += amount;
    }

    public void GrantXp(int xpAmount)
    {
        Xp += xpAmount;
        if (Xp > gameManager.XpToLevelUp(Lvl))
        {
            Xp = 0;
            OnLevelUp();
        }
        FloatingTextManager.instance.ShowFloatingText("+" + xpAmount + "xp", 12, Color.magenta, transform.position, "GetResource", 1.5f);
        gameManager.SaveGame();
        hud.onXpChange();
    }

    public void OnLevelUp()
    {
        // To not instantly show lvl up text on load 
        if (Time.time > 1)
            FloatingTextManager.instance.ShowFloatingText("Level Up!", 30, Color.magenta, transform.position, "GetResource", 1.5f);
        Lvl++;
        Hp = MaxHp;
        AbilityPoints++;
        AttributePoints++;
        hud.onHpChange();
        hud.onLevelChange();
    }

    public void EquipItem(Item item)
    {
        switch (item.type)
        {
            case "Weapon":
                Weapon weapon = (Weapon)item;
                this.Weapon = weapon;
                transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                break;
            case "Armor":
                Armor armor = (Armor)item;
                this.Armor = armor;
                transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                break;
            case "Helmet":
                Helmet helmet = (Helmet)item;
                this.Helmet = helmet;
                transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                break;
        }
    }

    public void UnequipItem(Item item)
    {
        switch (item.type)
        {
            // (Weapon cannot be unequipped)
            case "Armor":
                Armor = null;
                transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = null;
                break;
            case "Helmet":
                Helmet = null;
                transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                break;
        }
    }

    // Return the total amount of defense including items
    public int GetTotalDefense()
    {
        if (armor != null && helmet != null)
            return defense + armor.defense + helmet.defense;
        else if (armor == null && helmet != null)
            return defense + helmet.defense;
        else if (armor != null && helmet == null)
            return defense + armor.defense;
        else return defense;
    }

    // Return the total amount of attack power including items
    public int GetTotalAttackPower()
    {
        return attackPower + weapon.attackPower;
    }

    // Receive damage
    public void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            if (damageAmount > 0)
            {
                Hp -= damageAmount;
                FloatingTextManager.instance.ShowFloatingText(damageAmount.ToString(), 30, Color.red, origin, "Hit", 2.0f);
            }
            else FloatingTextManager.instance.ShowFloatingText("0", 30, Color.red, origin, "Hit", 2.0f);
            pushDirection = (transform.position - origin).normalized * pushForce;

            if (Hp <= 0)
            {
                Hp = 0;
                Death();
            }
        }
        hud.onHpChange();
    }

    public void InitializePlayer()
    {
        Items = new Item[15];

    }

    public void Death()
    {
        Debug.Log("Dead");
    }
}
