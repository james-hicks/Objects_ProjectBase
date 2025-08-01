using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyAstroidSmoke : EnemyAstroid
{
    [SerializeField] private GameObject areaEffectPrefab;
    [SerializeField] private float areaEffectDuration = 5f;
       

    // Track what caused the death
    private bool shouldSpawnSmoke = false;

    protected override void Start()
    {
        base.Start();
        health = new Health(10, 0);
    }

    // Override Update to prevent smoke on out-of-bounds death
    protected override void Update()
    {
        if (!hasCalculatedDirection || target == null) return;

        // Move in calculated direction
        transform.position += moveDirection * speed * Time.deltaTime;

        // Check for attack range (collision with player)
        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            shouldSpawnSmoke = true; // Player collision = spawn smoke
            Attack(attackTime);
            Die();
        }

        // Check if asteroid has moved too far away
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > destroyDistance)
            {
                shouldSpawnSmoke = false; // Out of bounds = no smoke
                Die();
            }
        }
    }

    // Override to handle damage from bullets
    public override void GetDamage(float damage)
    {
        shouldSpawnSmoke = true; // Bullet hit = spawn smoke
        base.GetDamage(damage);
    }

    // Override collision to ensure smoke spawns
    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null && player.health.GetCurrentHealth() > 0)
        {
            shouldSpawnSmoke = true; // Player collision = spawn smoke
            Attack(attackTime);
            Die();
        }
    }

    public override void Die()
    {
        // Only spawn smoke if death was caused by bullets or player
        if (shouldSpawnSmoke && areaEffectPrefab != null)
        {            
            AreaEffect.SpawnAreaEffect(areaEffectPrefab, transform.position, areaEffectDuration);
        }
       
        base.Die();
    }
}
