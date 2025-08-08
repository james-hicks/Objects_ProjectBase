using UnityEngine;

public class EnemyMelee : Enemy
{
    [Header("Melee Settings")]
    public float attackRange = 2f;
    public float attackTime = 1f;
    public float meleeDamage = 10f;
    private float attackTimer = 0f;

    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);
    }

    protected override void Update()
    {
        base.Update(); // This handles rotation

        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Attack if in range
        if (distanceToTarget <= attackRange)
        {
            Attack(attackTime);
        }
    }

    protected override void FixedUpdate()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            // Move towards target (player)
            MoveTowardsTarget();
        }
        else
        {
            // Stop when in attack range
            StopMovement();
        }
    }

    public override void Attack(float interval)
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= interval)
        {
            attackTimer = 0f;

            if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.GetDamage(meleeDamage);
                    Debug.Log($"Melee enemy dealt {meleeDamage} damage!");
                }
            }
        }
    }
}
