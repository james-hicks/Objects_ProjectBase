using UnityEngine;

public class EnemyPistolScatter : EnemyPistol
{
    

    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float spreadAngle = 45f;

    

    protected override void Start()
    {
        base.Start();
        health = new Health(20, 0);
    }

    protected override void Update()
    {
        if (player == null) return;
                
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, target.position) < shootingRange)
        {
            if (Time.time >= nextShotTime)
            {
                ScatterShot();
                nextShotTime = Time.time + shotRate;
            }
        }
    }

    private void ScatterShot()
    {
        // Calculate angle step between bullets
        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate bullet angle
            float currentAngle = startAngle + (angleStep * i);

            // Get the current rotation and add scatter offset
            Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, currentAngle);

            // Spawn bullet
            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            Bullet tempBullet = Instantiate(bulletPrefab, spawnPosition, bulletRotation);
            tempBullet.SetBullet(shotDamage, "Player", bulletSpeed);

            // Auto-destroy bullet after 5 seconds
            Destroy(tempBullet.gameObject, 5f);
        }
    }
}
