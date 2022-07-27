using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Collidable
{
    [SerializeField] private Player player;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            // Create a new damage object, and then sending it to the fighter we hit
            Damage dmg = new Damage
            {
                origin = transform.position,
                dmgAmount = player.GetWeapon().minDmg,
                pushForce = player.GetWeapon().pushForce

            };
            coll.SendMessage("RecieveDamage", dmg);
        }

    }
}
