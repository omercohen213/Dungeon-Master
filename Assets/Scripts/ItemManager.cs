using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Collectable
{
    public Item item;

    private void PickUp()
    {
        Debug.Log("Picked up " + item.itemName);
        InventoryManager.instance.Add(item);
        Destroy(gameObject);
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E))        
                PickUp();
        
    }
}
