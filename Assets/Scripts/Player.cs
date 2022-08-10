using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Player : Fighter
{
    [SerializeField] private float speed = 2;

    [SerializeField] private HUD hud;

    public PlayerData playerData = new PlayerData();

    // Resources
    public int gold { get; private set; }
    public int xp { get; private set; }
    public int lvl { get; private set; }
    public int mp { get; private set; }
    public int maxMp { get; private set; }
    public string playerName { get; private set; }
    public int hp { get; set; }
    public int maxHp { get; set; }

    // Damage immunity 
    protected float immuneTime = 1.0f;
    protected float lastImmune;

    // Inventory
    public Item [] items;
    public int lastItem; // Number of items the player has
    //public List<Item> items;
    public int equippedWeaponIndex;
    public int equippedArmorIndex;
    public int equippedHelmetIndex;

    private Weapon weapon;
    private Armor armor;
    private Helmet helmet;
    [SerializeField] private GameObject playerNameObj;

    private void Awake()
    {
        //playerData.Init(this);
        //inventoryManager.Init(this);
    }

    private void Update()
    {
        // Player name; Change player local scale on turning to sprite renderer change
        playerNameObj.transform.position = new Vector3(transform.position.x, transform.position.y - 0.12f);
        if (transform.localScale.x == -1)
            playerNameObj.transform.localScale = new Vector3(-1, 1, 1);
        else playerNameObj.transform.localScale = new Vector3(1, 1, 1);
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
            hp -= dmg.dmgAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            FloatingTextManager.instance.ShowFloatingText(dmg.dmgAmount.ToString(), 30, Color.red, dmg.origin, "Hit", 2.0f);

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

    public void GrantXp(int amount)
    {
        xp += amount;
        if (xp > GameManager.instance.XpToLevelUp(lvl))
        {
            xp = 0;
            OnLevelUp();
        }
        GameManager.instance.SaveGame();
        hud.onXpChange();
    }

    public void OnLevelUp()
    {
        // To not instantly show lvl up text on load 
        if (Time.time > 1)
            FloatingTextManager.instance.ShowFloatingText("Level Up!", 30, Color.magenta, transform.position, "GetResource", 1.5f);
        maxHp += 10;
        hp = maxHp;
        // get attribute points
        lvl++;
        hud.onHpChange();
        hud.onLevelChange();
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void SavePlayer()
    {
        // Rescources data
        playerData.playerName = playerName;
        playerData.lvl = lvl;
        playerData.xp = xp;
        playerData.gold = gold;
        playerData.position = transform.position; // scene
        playerData.hp = hp;
        playerData.maxHp = maxHp;
        playerData.mp = mp;
        playerData.maxMp = maxMp;

        // Inventory data
        playerData.items = items.ToArray();
        playerData.lastItem = lastItem;
        playerData.equippedWeaponIndex = equippedWeaponIndex;
        playerData.equippedArmorIndex = equippedArmorIndex;
        playerData.equippedHelmetIndex = equippedHelmetIndex;

        playerData.weapon = weapon; // Starts at index 0 of items array
        if (playerData.equippedHelmetIndex != -1)
            playerData.helmet = helmet;
        if (playerData.equippedArmorIndex != -1)
            playerData.armor = armor;
    }

    public void LoadPlayer(PlayerData playerData)
    {
        this.playerData = playerData;

        // Rescources data
        playerName = playerData.playerName;
        lvl = playerData.lvl;
        xp = playerData.xp;
        gold = playerData.gold;
        hp = playerData.hp;
        maxHp = playerData.maxHp;
        mp = playerData.mp;
        maxMp = playerData.maxMp;

        // Inventory data
        items = playerData.items.ToArray();
        lastItem = playerData.lastItem;
        equippedWeaponIndex = playerData.equippedWeaponIndex;
        equippedArmorIndex = playerData.equippedArmorIndex;
        equippedHelmetIndex = playerData.equippedHelmetIndex;

        // Items data              
        weapon = (Weapon)items[equippedWeaponIndex];       
        transform.Find("Weapon").GetComponent<SpriteRenderer>().sprite = items[equippedWeaponIndex].itemSprite;

        if (equippedArmorIndex != -1)
        {
            armor = (Armor)items[equippedArmorIndex];
            transform.Find("Armor").GetComponent<SpriteRenderer>().sprite = items[equippedArmorIndex].itemSprite;
        }
        if (equippedHelmetIndex != -1)
        {
            helmet = (Helmet)items[equippedHelmetIndex];
            transform.Find("Helmet").GetComponent<SpriteRenderer>().sprite = items[equippedHelmetIndex].itemSprite;
        }

        // Spawn point
        RectTransform portalRectTransform = GameObject.Find("SpawnPoint").GetComponent<RectTransform>();
        Transform portal = GameObject.Find("SpawnPoint").transform;
        float portalWidth = portalRectTransform.rect.width * 0.16f;
        float portalHeight = portalRectTransform.rect.height * 0.16f;
        transform.position = portal.position + new Vector3(portalWidth, -portalHeight / 3, 0);
    }

    public void InitializePlayer()
    {
        playerName = "IchBinSpite";
        items = new Item[15];
        Weapon firstWeapon = Resources.Load<Weapon>("Items/Ninja_Sword");
        items[0] = firstWeapon;
        //items = new List<Item>();
    }
}
