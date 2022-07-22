using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : Collidable
{
    //public string weaponName;
    //public int minDmg, maxDmg;
    //public int price;
    //public bool isEquiped;


    // Damage structure
    public int[] damage = { 10, 20, 30, 40, 50 };
    public float pushForce = 2.0f;

    // Upgrade
    public int weaponLvl = 0;
    public SpriteRenderer spriteRenderer;

    // On hit
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
                dmgAmount = damage[weaponLvl],
                pushForce = pushForce

            };
            coll.SendMessage("RecieveDamage", dmg);
        }
    }

    public void UpgradeWeapon()
    {
        weaponLvl++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLvl];
    }

    public void SetWeaponLvl(int lvl)
    {
        weaponLvl = lvl;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLvl];

    }
}
