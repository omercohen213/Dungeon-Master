using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Fighter
{
    // Drops
    [SerializeField] private int xpAmount = 10;
    [SerializeField] private GameObject itemDropPrefab;

    // Logic
    [SerializeField] private int id;
    private Player player;
    public float triggerLength = 1;
    public float chaseLength = 5;
    public float chaseSpeed = 1.5f;
    public float returnSpeed = 3.0f;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPos;
    public int hp;
    public int maxHp;
    public int damage;

    // Immunity 
    private const float DEFAULT_IMMUNE_TIME = 0.5f;
    protected float immuneTime;
    protected float lastImmune;

    [SerializeField] private List<Item> itemDrops = new List<Item>();

    // Hitbox
    public ContactFilter2D filter;
    private Collider2D[] hits = new Collider2D[10];

    // Hp Bar
    private RectTransform hpBarFrame;
    private RectTransform hpBar;
    private Text hpText;
    [SerializeField] private Vector3 offset;
    private Camera cam;

    protected override void Start()
    {
        base.Start();
        cam = Camera.main;

        // Initialize hp bar and text
        hpBar = transform.Find("HpBarCanvas/HpBarFrame/HpBar").GetComponent<RectTransform>();
        hpText = transform.Find("HpBarCanvas/HpBarFrame/HpText").GetComponent<Text>();
        hpBarFrame = transform.Find("HpBarCanvas/HpBarFrame").GetComponent<RectTransform>();
        hpText.text = hp + " / " + maxHp;
        float hpRatio = (float)hp / maxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);

        // Player
        player = GameObject.Find("Player").GetComponent<Player>();
        playerTransform = player.transform;
        startingPos = transform.position;
    }

    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position + offset);
        if (hpBarFrame.transform.position != pos)
            hpBarFrame.transform.position = pos;

        if (AbilitiesManager.instance.isAbility1Active())
        {
            immuneTime = 0.1f;
        }
        else
        {
            immuneTime = DEFAULT_IMMUNE_TIME;
        }
    }

    private void FixedUpdate()
    {
        // Is player in chasing range?
        if (Vector3.Distance(startingPos, playerTransform.position) < chaseLength)
        {
            if (!chasing)
                UpdateMotor(startingPos - transform.position, returnSpeed); // Go back to the starting position

            // Is player in trigger range?
            if (Vector3.Distance(startingPos, playerTransform.position) < triggerLength)
                chasing = true;

            if (chasing)
                if (!collidingWithPlayer)
                    UpdateMotor((0.75f * playerTransform.position - transform.position).normalized, chaseSpeed); // Go to player
                else UpdateMotor(startingPos - transform.position, returnSpeed); // Go back to the starting position
        }
        else
        {
            // Player is not in range anymore, go back to the starting position                   
            UpdateMotor(startingPos - transform.position, returnSpeed);
            chasing = false;
        }

        // Check for overlaps
        collidingWithPlayer = false;

        // Collision work
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
                collidingWithPlayer = true;

            // Array is not cleaned up so we do it by ourselves
            hits[i] = null;
        }
    }

    protected override void RecieveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            
            lastImmune = Time.time;
            hp -= dmg.dmgAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            FloatingTextManager.instance.ShowFloatingText(dmg.dmgAmount.ToString(), 30, new Color(0.98f, 0.37f, 0), dmg.origin, "Hit", 2.0f);

            if (hp <= 0)
            {
                hp = 0;
                Death();
            }
        }
        onHpChange();
    }

    public void onHpChange()
    {
        hpText.text = hp + " / " + maxHp;
        float hpRatio = (float)hp / (float)maxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);
    }

    protected override void Death()
    {
        gameObject.SetActive(false);
        player.GrantXp(xpAmount);
        DropItem();
        Invoke("Respawn", 3);
        foreach (Quest quest in player.activeQuests)
            if (quest.enemiesIds.Contains(id))
                QuestManager.instance.UpdateActiveQuest(quest);
    }

    private void DropItem()
    {
        int rnd = Random.Range(0, 100);
        itemDrops.Sort((item1, item2) => item1.dropRate.CompareTo(item2.dropRate)); // Sort the list by drop rate (ascending order)

        // Drop the most rare item possible
        if (rnd <= itemDrops[0].dropRate)
            CreateItemDropGo(0);

        float lastDropRate = itemDrops[0].dropRate;
        for (int i = 1; i < itemDrops.Count; i++)
        {
            if (rnd >= itemDrops[i - 1].dropRate && rnd <= itemDrops[i].dropRate + lastDropRate)
            {
                CreateItemDropGo(i);
                return;
            }
            else lastDropRate = itemDrops[i].dropRate;
        }
    }

    // Create the item drop object and its components
    private void CreateItemDropGo(int index)
    {
        Item item = itemDrops[index];
        GameObject itemGo = Instantiate(itemDropPrefab);
        itemGo.GetComponent<ItemManager>().item = item;
        itemGo.GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        itemGo.name = item.name;
        itemGo.transform.position = transform.position;
        itemGo.transform.localScale = item.spriteSize;
        ItemManager.instance.DestroyItemDrop(); // Destroy object after a few seconds
    }

    private void Respawn()
    {
        Enemy enemyClone = Instantiate(this);
        enemyClone.name = name;
        enemyClone.transform.position = startingPos;
        enemyClone.hp = maxHp;
        enemyClone.gameObject.SetActive(true);

        Destroy(gameObject);
    }

}
