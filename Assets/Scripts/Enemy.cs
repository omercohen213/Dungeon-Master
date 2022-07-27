using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fighter
{

    // Drops
    [SerializeField] private int xpValue = 10;
    public GameObject itemDrop;
    private Player player;     

    // Logic
    public float triggerLength = 1;
    public float chaseLength = 5;
    public float chaseSpeed = 1.5f;
    public float returnSpeed = 3.0f;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPos;
    [SerializeField] private List<Item> itemDrops = new List<Item>();
   

    // Hitbox
    public ContactFilter2D filter;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        player = Player.instance;
        playerTransform = GameObject.Find("Player").transform;
        startingPos = transform.position;
        
 
    }

    private void FixedUpdate()
    {      
        // Is player in chasing range?
        if (Vector3.Distance(startingPos, playerTransform.position) < chaseLength)
        {
            if (!chasing)
                UpdateMotor(startingPos - transform.position, returnSpeed);

            // Is player in trigger range?
            if (Vector3.Distance(startingPos, playerTransform.position) < triggerLength)
                chasing = true;

            if (chasing)
                if (!collidingWithPlayer)
                    UpdateMotor((playerTransform.position - transform.position).normalized, chaseSpeed); // Go to player
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

    protected override void Death()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
        player.GrantXp(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 20, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
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
                // Creating the item object and its components
                GameObject itemDropClone = Instantiate(itemDrop);              
                itemDropClone.GetComponent<ItemManager>().SetItem(item);
                itemDropClone.GetComponent<SpriteRenderer>().sprite = item.itemSprite;
                itemDropClone.name = itemDrop.name;
                itemDropClone.transform.position = transform.position;
                itemDropClone.transform.localScale = item.spriteSize;

                Debug.Log(rnd + " Dropped: " + item.itemName);
                ItemManager.instance.DestroyItemDrop();
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
