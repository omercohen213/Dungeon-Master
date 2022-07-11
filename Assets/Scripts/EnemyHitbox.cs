using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHitbox : Collidable
{
    // Damage
    public int damage= 10;
    public float pushForce;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            // Create a new damage object
            Damage dmg = new Damage
            {
                origin = transform.position,
                dmgAmount = damage,
                pushForce = pushForce

            };
            coll.SendMessage("RecieveDamage", dmg);
        }

    }

}
