using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : Collidable
{
    private Item item;
    private readonly float timeBeforeDestroyed = 5f;

    public Item Item { get => item; set => item = value; }

    protected override void Start()
    {
        base.Start();
        Invoke("DestroyItemDrop", timeBeforeDestroyed);
    } 

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E))
                if (!Inventory.instance.isFull())
                    OnPickUp();
                else Debug.Log("No space in inventory!");
    }

    public void OnPickUp()
    {
        Inventory.instance.AddItem(Item);
        InventoryUI.instance.UpdateInventory();
        Destroy(gameObject);
    }

    public void DestroyItemDrop()
    {
        Destroy(gameObject);
    }
}
