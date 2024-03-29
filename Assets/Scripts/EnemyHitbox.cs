using UnityEngine;



public class EnemyHitbox : Collidable
{
    private Player player;
    [SerializeField] private Enemy enemy;
    public float pushForce;

    protected override void Start()
    {
        base.Start();
        player = Player.instance;
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            float rnd = Random.Range(0.8f, 1);
            int damageAmount = Mathf.RoundToInt(enemy.damage * rnd - player.GetTotalDefense());
            Vector3 origin = transform.position;

            IDamageable damageable = coll.gameObject.GetComponent<IDamageable>();
            damageable.ReceiveDamage(damageAmount, pushForce, origin);
        }
    }
}
