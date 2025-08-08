using UnityEngine;

public class EnemyPistol : Enemy
{
    [Header("Shooting Settings")]
    public float shootingRange = 8f;
    public float shotDamage = 5f;
    public float shotRate = 1f;
    public float weaponDamage = 1f;
    public float bulletSpeed = 10f;
    public Bullet bulletPrefab;

    private float nextShotTime = 0f;

    protected override void Start()
    {
        base.Start();
        health = new Health(30, 0);
        weapon = new Weapon("EnemyWeapon", shotDamage, bulletSpeed);
    }

    protected override void Update()
    {
        base.Update(); // This handles rotation

        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Shoot if in range and ready
        if (distanceToTarget <= shootingRange && Time.time >= nextShotTime)
        {
            Attack(0f);
            nextShotTime = Time.time + shotRate;
        }
    }

    protected override void FixedUpdate()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > shootingRange)
        {
            // Move towards target (player) until in shooting range
            MoveTowardsTarget();
        }
        else
        {
            // Stop when in shooting range
            StopMovement();
        }
    }

    public override void Attack(float shootingRate)
    {
        if (weapon != null && bulletPrefab != null)
        {
            weapon.Shoot(bulletPrefab, "Player", this);
            Debug.Log("Enemy fired a shot!");
        }
    }
}
