using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Fighter
{
    // Drops
    private ItemDropManager itemDropManager;
    [SerializeField] private int xpAmount = 10;
    [SerializeField] private List<Item> itemDrops = new List<Item>();

    // Logic
    [SerializeField] private int id;
    [SerializeField] private string enemyName;
    private Player player;
    public float triggerLength = 1;
    public float chaseLength = 5;
    public float chaseSpeed = 1.5f;
    public float returnSpeed = 3.0f;
    private bool isChasing;
    private bool isCollidingWithPlayer;
    private Vector3 startingPos;
    public int hp;
    public int maxHp;
    public int damage;
    private readonly float respawnTimer = 3f;
    private const float AA_DAMAGE_DELAY_TIME = 0.5f;
    private const float ABILITY_DAMAGE_DELAY_TIME = 0.1f;

    // Hitbox
    public ContactFilter2D filter;
    private readonly Collider2D[] hits = new Collider2D[10];    

    // Name and hp Bar
    private readonly float offset = 0.12f;
    private RectTransform hpBarFrame;
    private RectTransform hpBar;
    private Text hpText;
    private GameObject enemyNameGo;
    private Camera cam;

    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
        itemDropManager = ItemDropManager.instance;

        // Initialize hp bitialize hp bar and text
        hpBar = transform.Find("HpBarCanvas/HpBarFrame/HpBar").GetComponent<RectTransform>();
        hpText = transform.Find("HpBarCanvas/HpBarFrame/HpText").GetComponent<Text>();
        hpBarFrame = transform.Find("HpBarCanvas/HpBarFrame").GetComponent<RectTransform>();
        hpText.text = hp + " / " + maxHp;
        float hpRatio = (float)hp / maxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);

        // Player
        player = GameObject.Find("Player").GetComponent<Player>();
        startingPos = transform.position;
        enemyNameGo = transform.Find("EnemyNameCanvas/EnemyName").gameObject;
        enemyNameGo.GetComponent<Text>().text = enemyName;
        enemyNameGo.GetComponent<Text>().color = Color.red;
    }

    private void Update()
    {
        Vector3 namePos = cam.WorldToScreenPoint(transform.position + new Vector3(0, offset));
        if (enemyNameGo.transform.position != namePos)
            enemyNameGo.transform.position = namePos;

        Vector3 hpBarPos = cam.WorldToScreenPoint(transform.position + new Vector3(0, -offset));
        if (hpBarFrame.transform.position != hpBarPos)
            hpBarFrame.transform.position = hpBarPos;

        if (AbilitiesManager.instance.IsAbilityActive("GarenE"))       
            damageDelay = ABILITY_DAMAGE_DELAY_TIME;
        else
        damageDelay = AA_DAMAGE_DELAY_TIME;
    }

    private void FixedUpdate()
    {
        // Is player in chasing range?
        if (Vector3.Distance(startingPos, player.transform.position) < chaseLength)
        {
            if (!isChasing)
                UpdateMotor(startingPos - transform.position, returnSpeed); // Go back to the starting position

            // Is player in trigger range?
            if (Vector3.Distance(startingPos, player.transform.position) < triggerLength)
                isChasing = true;

            if (isChasing)
                if (!isCollidingWithPlayer)
                    UpdateMotor((0.75f * player.transform.position - transform.position).normalized, chaseSpeed); // Go to player
                else UpdateMotor(startingPos - transform.position, returnSpeed); // Go back to the starting position
        }
        else
        {
            // Player is not in range anymore, go back to the starting position                   
            UpdateMotor(startingPos - transform.position, returnSpeed);
            isChasing = false;
        }

        // Check for overlaps
        isCollidingWithPlayer = false;

        // Collision work
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
                isCollidingWithPlayer = true;

            // Array is not cleaned up so we do it by ourselves
            hits[i] = null;
        }
    }

    public void OnHpChange()
    {
        hpText.text = hp + " / " + maxHp;
        float hpRatio = (float)hp / maxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);
    }

    // Respawn the enemy object after death
    private void Respawn()
    {
        hp = maxHp;
        OnHpChange();
        transform.position = startingPos;
        gameObject.SetActive(true);
    }

    public override void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin)
    {
        player.IsInCombat = true;
        if (Time.time - lastDamage > damageDelay)
        {
            lastDamage = Time.time;
            if (damageAmount > 0)
            {
                hp -= damageAmount;
                FloatingTextManager.instance.ShowFloatingText(damageAmount.ToString(), 30, new Color(0.98f, 0.37f, 0), origin, "Hit", 2.0f);
            }
            else FloatingTextManager.instance.ShowFloatingText("0", 30, new Color(0.98f, 0.37f, 0), origin, "Hit", 2.0f);
            pushDirection = (transform.position - origin).normalized * pushForce;

            if (hp <= 0)
            {
                hp = 0;
                Death();
            }
        }
        OnHpChange();
    }  

    // Calculate which item the enemy should drop
    public void DropItem()
    {
        int rnd = Random.Range(0, 100);
        itemDrops.Sort((item1, item2) => item1.dropRate.CompareTo(item2.dropRate)); // Sort the list by drop rate (ascending order)

        // Drop the most rare item possible
        if (rnd <= itemDrops[0].dropRate)
            itemDropManager.CreateItemDrop(itemDrops[0], transform.position);

        float lastDropRate = itemDrops[0].dropRate;
        for (int i = 1; i < itemDrops.Count; i++)
        {
            if (rnd >= itemDrops[i - 1].dropRate && rnd <= itemDrops[i].dropRate + lastDropRate)
            {
                itemDropManager.CreateItemDrop(itemDrops[i], transform.position);
                return;
            }
            else lastDropRate = itemDrops[i].dropRate;
        }
    }

    public override void Death()
    {
        gameObject.SetActive(false);
        player.GrantXp(xpAmount);
        DropItem();
        Invoke(nameof(Respawn), respawnTimer);
        foreach (Quest quest in player.ActiveQuests)
        {
            if (quest.enemiesIds.Contains(id))
                QuestManager.instance.UpdateActiveQuest(quest);
        }
    }    

    // Check if given damage is enough to kill enemy
    public override bool IsDamageToKill(float damage)
    {
        return damage > hp;
    }
}

