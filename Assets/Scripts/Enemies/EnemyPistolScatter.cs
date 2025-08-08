using UnityEngine;

public class EnemyPistolScatter : EnemyPistol
{
    [Header("Scatter Shot Settings")]
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float spreadAngle = 45f;

    // Make nextShotTime accessible to this child class
    private float scatterNextShotTime = 0f;

    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);
    }

    protected override void Update()
    {
        // Use base rotation handling
        if (target == null) return;

        // Always rotate towards target
        RotateTowardsTarget();

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Shoot scatter shot if in range and ready
        if (distanceToTarget <= shootingRange && Time.time >= scatterNextShotTime)
        {
            ScatterShot();
            scatterNextShotTime = Time.time + shotRate;
        }
    }

    protected override void FixedUpdate()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > shootingRange)
        {
            // Move towards target until in shooting range
            MoveTowardsTarget();
        }
        else
        {
            // Stop when in shooting range
            StopMovement();
        }
    }

    private void ScatterShot()
    {
        if (bulletPrefab == null) return;

        // Calculate angle step between bullets
        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate bullet angle
            float currentAngle = startAngle + (angleStep * i);

            // Get the current rotation and add scatter offset
            Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, currentAngle);

            // Spawn bullet at enemy position
            Vector3 spawnPosition = transform.position;
            Bullet tempBullet = Instantiate(bulletPrefab, spawnPosition, bulletRotation);
            tempBullet.SetBullet(shotDamage, "Player", bulletSpeed);

            // Auto-destroy bullet after 5 seconds
            Destroy(tempBullet.gameObject, 5f);
        }

        Debug.Log($"Scatter shot fired {bulletCount} bullets!");
    }

    // Override the base Attack method to use scatter shot
    public override void Attack(float shootingRate)
    {
        ScatterShot();
    }
}
