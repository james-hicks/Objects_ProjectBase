using UnityEngine;
using System;

public class EnemyAstroid : Enemy
{
    private const string PLAYER_TAG = "Player";
    private float timer = 0;
    public float meleeDamage;
    public float attackrange;
    public float attackTime;
    public float destroyDistance = 10f;

    public Transform player;
    private Vector3 moveDirection;
    private bool hasCalculatedDirection = false;

    protected override void Start()
    {
        base.Start();
        health = new Health(10, 0);

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.angularVelocity = UnityEngine.Random.Range(-90f, 90f); // Random spin
        }

        // Find Player Location and calculate straight-line direction
        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;
            hasCalculatedDirection = true;
        }
        else
        {            
            Destroy(gameObject);
            return;
        }

        // Also get player transform reference
        GameObject playerGO = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        if (playerGO != null)
        {
            player = playerGO.transform;
        }
    }

    protected override void Update()
    {
        if (!hasCalculatedDirection || target == null) return;

        // ONLY move in the calculated straight line direction
        transform.position += moveDirection * speed * Time.deltaTime;

        // Check for attack range
        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            Attack(attackTime);
            Destroy(gameObject);
        }

        // Check if asteroid has moved too far away
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > destroyDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    protected new void FixedUpdate()
    {
        
    }



    public override void Attack(float interval)
    {
        // Damage on contact
        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            target.GetComponent<IDamageable>().GetDamage(meleeDamage);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null && player.health.GetCurrentHealth() > 0)
        {            
            Attack(attackTime);
            Destroy(gameObject);
        }
    }
}
