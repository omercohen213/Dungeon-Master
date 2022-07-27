using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : Fighter
{
    public static Player instance;
    
    // Stats
    [SerializeField] private float speed = 2;
    

    [SerializeField] private HUD hud;

    // Resources
    private int gold; // {get; set;}
    private int xp;
    private int lvl;
    private int mp;
    private int maxMp;

    [SerializeField] private Weapon weapon;
    [SerializeField] private Armor armor;


    [SerializeField] private List<int> xpTable;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
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
        base.RecieveDamage(dmg);
        hud.onHpChange();
    }

    public void SetLevel(int lvl)
    {
        for (int i = 0; i < lvl; i++)
        {
            OnLevelUp();
        }
    }
    public int GetGold()
    {
        return this.gold;
    }
    public void SetGold(int gold)
    {
        this.gold = gold;
    }
    public int GetHp()
    {
        return this.hp;
    }
    public int GetMaxHp()
    {
        return this.maxHp;
    }
    public int GetMp()
    {
        return this.mp;
    }
    public int GetMaxMp()
    {
        return this.maxMp;
    }
    public int GetXp()
    {
        return this.xp;
    }
    public int GetLevel()
    {
        int lvl = 0;
        int add = 0;
        while (xp >= add)
        {
            add += xpTable[lvl];
            lvl++;
            if (lvl == xpTable.Count)
                return lvl;

        }
        return lvl;
    }
    public List<int> GetXpTable()
    {
        return xpTable;
    }
    public int GetXpToLevelUp(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }

        return xp;
    }
    public void SetXp(int xp)
    {
        this.xp = xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetLevel();
        this.xp += xp;
        if (currLevel < GetLevel())
            OnLevelUp();
        hud.onXpChange();
    }
    public void OnLevelUp()
    {
        // To not instantly show text on load 
        if (Time.time > 1)
            GameManager.instance.ShowText("Level Up!", 25, Color.magenta, transform.position, Vector3.up * 25, 2.0f);
        maxHp += 10;
        hp = maxHp;
        hud.onHpChange();
        hud.onLevelChange();
    }  
    public Weapon GetWeapon()
    {
        return weapon;
    }
    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

}
