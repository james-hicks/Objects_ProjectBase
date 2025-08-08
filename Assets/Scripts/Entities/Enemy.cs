using System;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : PlayableObject
{
    public EnemyType enemyType;

    protected Transform target;
    [SerializeField] protected float speed;

    [Header("Movement Settings")]
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected bool useRigidbodyMovement = true;

    protected Rigidbody2D rb;

    public int ScoreValue;

    [Header("Particle System on Death")]
    [SerializeField] ParticleSystem onDeathExplosion;

    private ParticleSystem explosionInstance;

    protected virtual void Start()
    {
        try
        {
            target = FindFirstObjectByType<Player>().GetComponent<Transform>();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("There is no Alive player in the game, stopping all future spawning." + e);

            EnemySpawning enemySpawning = FindFirstObjectByType<EnemySpawning>();
            if (enemySpawning != null)
            {
                enemySpawning.StopSpawning();
            }

            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody2D>();

        // Ensure rigidbody is set up properly for 2D movement
        if (rb != null)
        {
            rb.freezeRotation = true; // Prevent physics rotation interference
            rb.gravityScale = 0; // No gravity for space enemies
        }
    }

    protected virtual void Update()
    {
        if (target == null) return;

        // Always update rotation to face player
        RotateTowardsTarget();
    }

    // Separate rotation logic that always runs
    protected virtual void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Smooth rotation towards target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // This will be overridden by specific enemy types
    protected virtual void FixedUpdate()
    {
        if (target == null) return;
        // Base movement - override in child classes for specific behavior
    }

    public override void Move(Vector2 direction, Vector2 target) { }

    public override void Move(float spd)
    {
        transform.Translate(Vector2.right * spd * Time.deltaTime);
    }

    // Improved movement method
    public override void Move(Vector2 direction)
    {
        if (rb == null) return;

        if (useRigidbodyMovement)
        {
            // Use Rigidbody for physics-based movement
            Vector2 targetDirection = direction.normalized;
            rb.linearVelocity = targetDirection * speed;
        }
        else
        {
            // Direct transform movement (ignores colliders)
            transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
        }
    }

    // Helper method for movement towards target
    protected virtual void MoveTowardsTarget(float maxDistance = float.MaxValue)
    {
        if (target == null || rb == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > maxDistance) return;

        Vector2 direction = (target.position - transform.position).normalized;

        if (useRigidbodyMovement)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    // Stop movement
    protected virtual void StopMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public override void Shoot()
    {
        Debug.Log("Shooting");
    }

    public override void Attack(float interval)
    {
        Debug.Log("Attacking");
    }

    public override void Die()
    {
        spawnParticles();

        GameManager gameManager = GameManager.GetInstance();
        if (gameManager != null && gameManager.scoreManager != null)
        {
            gameManager.scoreManager.IncrementScore(ScoreValue);
        }

        EnemySpawning enemySpawning = FindFirstObjectByType<EnemySpawning>();
        if (enemySpawning != null)
        {
            enemySpawning.NotifyEnemyDeath(transform.position);
        }

        Destroy(gameObject);
    }

    public void SetEnemyType(EnemyType enemyType)
    {
        this.enemyType = enemyType;
    }

    private void spawnParticles()
    {
        if (onDeathExplosion != null)
        {
            explosionInstance = Instantiate(onDeathExplosion, transform.position, quaternion.identity);
            Destroy(explosionInstance.gameObject, 3f);
        }
    }
}
