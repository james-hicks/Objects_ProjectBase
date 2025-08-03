using UnityEngine;

public class ExploderEnemy : Enemy
{
    [Header("Exploder Settings")]
    public float explodeRange; // Set range at which enemy explodes
    [SerializeField] private float explosionRadius = 5f; // radius in which explosion will deal damage
    [SerializeField] private float explodeDamage = 5f;
    [SerializeField] private int baseHealth = 4;
    [SerializeField] private GameObject explosionPrefab;

    private bool hasExploded = false;

    protected override void Start()
    {
        base.Start();
        health = new Health(baseHealth, 0); // Init health
    }

    protected override void Update()
    {
        // Exit if player does not exist (has died)
        if (target == null || target.Equals(null)) return;

        // get distance to player
        float distance = Vector2.Distance(transform.position, target.position);

        // if not within explodeRange, move
        if (distance > explodeRange / 2)
        {
            base.Update(); // Continue moving
        }
        // if within explodeRange, explode
        if (distance < explodeRange)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // do not explode twice
        if (hasExploded) return;
        // set hasExploded to true
        hasExploded = true;

        // Spawn explosion
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Find all colliders in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        // Loop through them
        foreach (Collider2D hit in hits)
        {
            // if it's a player or an enemy
            if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
            {
                // deal damage within the explosionRadius
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.GetDamage(explodeDamage);
                }
            }
        }

    }

    // Extend Enemy Die() to explode when shot
    public override void Die()
    {
        // when shot, check if exploded, if not already the case, explode
        if (!hasExploded) {
            Explode();
        }

        // Implement Enemy base.Die()
        base.Die();
    }
}
