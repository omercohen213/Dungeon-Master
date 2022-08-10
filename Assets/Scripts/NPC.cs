
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Collidable
{
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E))
                FloatingTextManager.instance.ShowFloatingText("Hello", 30, Color.white , transform.position, null, 5.0f);
    }
}
