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
            Damage dmg = new Damage();
            dmg.origin = transform.position;
            float rnd = Random.Range(0.8f, 1);
            dmg.dmgAmount = Mathf.RoundToInt(player.AttackPower * rnd);
            dmg.pushForce = player.GetWeapon().pushForce;

            coll.SendMessage("RecieveDamage", dmg);
        }

    }
}
