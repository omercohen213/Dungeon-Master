using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fighter
{

    // Drops
    [SerializeField] private int xpValue = 10;
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private Player player; // DETECT FOR ANY PLAYER

    // Logic
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

    // Immunity 
    protected float immuneTime = 0.5f;
    protected float lastImmune;

    [SerializeField] private List<Item> itemDrops = new List<Item>();
   

    // Hitbox
    public ContactFilter2D filter;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        playerTransform = player.transform;
        startingPos = transform.position;       
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
            
            if (hits[i].tag=="Fighter" && hits[i].name == "Player")
                collidingWithPlayer=true;

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
    }

    protected override void Death()
    {
        gameObject.SetActive(false);
        player.GrantXp(xpValue);
        FloatingTextManager.instance.ShowFloatingText("+" + xpValue + "xp", 12, Color.magenta, player.transform.position, "GetResource", 1.5f);
        DropItem();
        Invoke("Respawn", 3);
    }

    private void DropItem()
    {      
        int rnd = Random.Range(0, 100);
        itemDrops.Sort((item1, item2) => item1.dropRate.CompareTo(item2.dropRate)); // Sort the list by drop rate (ascending order)
        
        // Drop the most rare item possible (try other implementations)
        foreach (Item item in itemDrops){
            if (rnd < item.dropRate)
            { 
                // Drop item; Create the item object and its components
                GameObject itemDrop = Instantiate(itemDropPrefab);              
                itemDrop.GetComponent<ItemManager>().item = item;
                itemDrop.GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                itemDrop.name = item.name;
                itemDrop.transform.position = transform.position;
                itemDrop.transform.localScale = item.spriteSize;

                ItemManager.instance.DestroyItemDrop(); // Destroy object after a few seconds
                return;
            }
        }
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
