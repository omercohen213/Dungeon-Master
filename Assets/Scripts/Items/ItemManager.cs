using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Collectable
{
    
    public static ItemManager instance;

    [SerializeField] private Item item;

    private IEnumerator coroutine;
    private float timeBeforeDestroyed = 10;

    private void Awake()
    {
        instance = this;
    }
    private void PickUp()
    {
        Debug.Log("Picked up " + item.itemName);
        InventoryManager.instance.AddItem(item);
        Destroy(gameObject);
       
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E)) {
                if (!InventoryManager.instance.isFull())
                    PickUp();
                else Debug.Log("No space in inventory!");
            }
    }

    // Destroy item game object after dropped
    private IEnumerator DestroyItemDrop(float waitTime, GameObject gameObject)
    {
        
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
        Debug.Log("Destroyed " + gameObject.name);
    }
    public IEnumerator DestroyItemDrop()
    {
        coroutine = DestroyItemDrop(timeBeforeDestroyed, gameObject);
        StartCoroutine(coroutine);
        return DestroyItemDrop(timeBeforeDestroyed, gameObject);
    }
    
    public Item GetItem()
    {
        return item;
    }
    public void SetItem(Item item)
    {
        this.item = item;
    }
}
