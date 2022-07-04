using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    // Damage structure
    public int damage= 1;
    public float pushForce = 2.0f;

    // Upgrade
    public int weaponLvl = 1;
    private SpriteRenderer spriteRenderer;

    // Swing
    private float cooldown = 0.5f; //time between attacks
    private float lastSwing;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // Swing the weapon when not on cooldown
        if (Input.GetKeyDown(KeyCode.Space))
            if (Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
        /* Change weapon when pressing on numbers
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {

        }
        */
        
    }

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
                dmgAmount = damage,
                pushForce = pushForce

            };
            coll.SendMessage("RecieveDamage", dmg);
        }
    }

    // Swinging animation and change boxCollider position 
    private void Swing()
    {
        Debug.Log("swing");
    }
}
