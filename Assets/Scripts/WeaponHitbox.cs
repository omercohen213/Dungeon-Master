using UnityEngine;

public class WeaponHitbox : Collidable
{
    private Player player;
    private bool hitVFXInstantiated = false;
    private readonly float hitVFXScale = 0.1f;


    protected override void Start()
    {
        base.Start();
        player = Player.instance;
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.CompareTag("Fighter"))
        {
            if (coll.name == "Player")
                return;

            float rnd = Random.Range(0.8f, 1);
            int damageAmount = Mathf.RoundToInt(player.GetTotalAttackPower() * rnd);
            float pushForce = player.Weapon.pushForce;
            Vector3 origin = transform.position;

            Fighter fighter = coll.GetComponent<Fighter>();
            if (!hitVFXInstantiated)
            {
                fighter.ShowHitVFX(origin, hitVFXScale, coll.transform);
                hitVFXInstantiated = true;
            }
            hitVFXInstantiated = false;

            IDamageable damageable = coll.gameObject.GetComponent<IDamageable>();
            damageable.ReceiveDamage(damageAmount, pushForce, origin);
        }
    }
}
