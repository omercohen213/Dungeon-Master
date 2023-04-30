using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHitbox : Collidable
{
    private Player player;
    [SerializeField] private Enemy enemy;
    public float pushForce;

    protected override void Start()
    {
        base.Start();
        player = GameObject.Find("Player").GetComponent<Player>() ;
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            // Create a new damage object
            Damage dmg = new Damage();
            dmg.origin = transform.position;
            float rnd = Random.Range(0.8f, 1);
            dmg.dmgAmount = Mathf.RoundToInt(enemy.damage * rnd - player.Defense);
            dmg.pushForce = pushForce;
            coll.SendMessage("RecieveDamage", dmg);
        }
    }
}
