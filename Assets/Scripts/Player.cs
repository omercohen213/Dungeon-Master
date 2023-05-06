using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Fighter, IDamageable
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

    // Hp and Mp regeneration
    private float hpRegenTimer;
    private float hpRegenDelay = 3f;
    private readonly int hpRegen = 10;
    private float mpRegenTimer;
    private float mpRegenDelay = 1f;
    private readonly int mpRegen = 5;
    private float inCombatTimer;
    private bool isInCombat = false;
    private readonly float inCombatDelay = 5f;
    public bool IsInCombat {set => isInCombat = value; }

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

    private List<Quest> activeQuests;
    public List<Quest> ActiveQuests { get => activeQuests; set => activeQuests = value; }

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
        hpRegenTimer = 0;
        mpRegenTimer = 0;
        inCombatTimer = 0;
    }

    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position + offset);
        if (playerNameGo.transform.position != pos)
            playerNameGo.transform.position = pos;
        if (!isInCombat)
        {
            RegenerateHpAndMp();
        }
        else
        {
            UpdateCombatTimer();
        }
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
        string text = "+" + amount + " gold!";
        int fontSize = 20;
        float destroyTimer = 1.5f;
        FloatingTextManager.instance.ShowFloatingText(text, fontSize, Color.yellow, transform.position, "GetResource", destroyTimer);
        GameManager.instance.SaveGame();
        hud.onGoldChange();
    }

    public void GrantXp(int xpAmount)
    {
        Xp += xpAmount;
        if (Xp > gameManager.XpToLevelUp(Lvl))
        {
            Xp = 0;
            OnLevelUp();
        }
        string text = "+" + xpAmount + "xp";
        int fontSize = 20;
        float destroyTimer = 1.5f;
        FloatingTextManager.instance.ShowFloatingText(text, fontSize, Color.magenta, transform.position, "GetResource", destroyTimer);
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
        AttributePoints += 3;
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
                transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = null;
                break;
        }
    }

    // Regenerate Hp and Mp with corresponding regen
    public void RegenerateHpAndMp()
    {
        // Player is dead
        /*if (hp <= 0)
        {
            return;
        }*/

        // Check if both hp and mp need to be regenerated
        bool shouldRegenerateHp = hp < maxHp;
        bool shouldRegenerateMp = mp < maxMp;

        if (!shouldRegenerateHp && !shouldRegenerateMp)
        {
            return;
        }

        if (shouldRegenerateHp)
        {
            hpRegenTimer += Time.deltaTime;
            if (hpRegenTimer >= hpRegenDelay)
            {
                hp += hpRegen;
                if (hp > maxHp)
                {
                    hp = maxHp;
                }
                hud.onHpChange();
                hpRegenTimer = 0f;
            }
        }

        if (shouldRegenerateMp)
        {
            mpRegenTimer += Time.deltaTime;
            if (mpRegenTimer >= mpRegenDelay)
            {
                mp += mpRegen;
                if (mp > maxMp)
                {
                    mp = maxMp;
                }
                hud.onMpChange();
                mpRegenTimer = 0f;
            }
        }
    }

    // Check if player stopped combat
    private void UpdateCombatTimer()
    {
        inCombatTimer -= Time.deltaTime;
        if (inCombatTimer <= 0f)
        {
            isInCombat = false;
            inCombatTimer = 0f;
        }
    }
  
    // Receive damage
    public void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin)
    {
        isInCombat = true;
        inCombatTimer = inCombatDelay;

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

    public bool HasHelmet()
    {
        return (InventoryManager.instance.EquippedHelmetIndex != -1);
    }
    public bool HasArmor()
    {
        return (InventoryManager.instance.EquippedArmorIndex != -1);
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
    // Initialize needed variables on game start
    public void InitializePlayer()
    {
        items = new Item[15];
        activeQuests = new List<Quest>();
    }
    public void Death()
    {
        Debug.Log("Dead");
    }
}
