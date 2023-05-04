using UnityEngine;

public class WeaponHitbox : Collidable
{
    private Player player;

    protected override void Start()
    {
        base.Start();
        player = Player.instance;
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            float rnd = Random.Range(0.8f, 1);
            int damageAmount = Mathf.RoundToInt(player.GetTotalAttackPower() * rnd);
            float pushForce = player.Weapon.pushForce;
            Vector3 origin = transform.position;

            IDamageable damageable = coll.gameObject.GetComponent<IDamageable>();
            damageable.ReceiveDamage(damageAmount, pushForce, origin);
        }

    }
}
