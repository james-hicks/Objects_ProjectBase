using UnityEngine;

public class EnemyExploder : EnemyMelee
{
    [Header("Explosion Settings")]
    public float blastRadius = 5f;

    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);
    }

    protected override void Update()
    {
        
        base.Update();

        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Attack and explode when in range
        if (distanceToTarget <= attackRange)
        {
            Attack(attackTime);
            ExplodeAndDie();
        }
    }

   

    public override void Attack(float interval)
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.GetDamage(meleeDamage);
                Debug.Log($"Exploder enemy dealt {meleeDamage} damage!");
            }
        }
    }

    private void ExplodeAndDie()
    {
        // Create explosion effect
        CreateExplosion();

        // Destroy this enemy
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        // Find all colliders in blast radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Damage player if in range
            if (hitCollider.CompareTag("Player"))
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.GetDamage(meleeDamage * 2f); // Explosion does double damage                    
                }
            }
        }
    }
}

    
