using UnityEngine;

public class EnemyAstroidSmoke : EnemyAstroid
{
    [Header("Smokescreen Settings")]
    [SerializeField] private GameObject smokescreenPrefab;
    [SerializeField] private float smokescreenDuration = 6f;
    [SerializeField] private Vector3 smokescreenOffset = Vector3.zero;

    [Header("Spawn Conditions")]
    [SerializeField] private bool spawnOnPlayerKill = true;
    [SerializeField] private bool spawnOnBulletKill = true;
    [SerializeField] private bool spawnOnBoundaryKill = false;

    private bool shouldSpawnSmokescreen = false;

    protected override void Start()
    {
        base.Start();
        health = new Health(10, 0);
    }

    protected override void Update()
    {
        if (!hasCalculatedDirection || target == null) return;

        transform.position += moveDirection * speed * Time.deltaTime;

        if (Vector2.Distance(transform.position, target.position) < attackrange)
        {
            if (spawnOnPlayerKill)
            {
                shouldSpawnSmokescreen = true;
            }
            Attack(attackTime);
            Die();
        }

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > destroyDistance)
            {
                if (spawnOnBoundaryKill)
                {
                    shouldSpawnSmokescreen = true;
                }
                Die();
            }
        }
    }

    public override void GetDamage(float damage)
    {
        if (spawnOnBulletKill)
        {
            shouldSpawnSmokescreen = true;
        }
        base.GetDamage(damage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null && player.health.GetCurrentHealth() > 0)
        {
            if (spawnOnPlayerKill)
            {
                shouldSpawnSmokescreen = true;
            }
            Attack(attackTime);
            Die();
        }
    }

    public override void Die()
    {
        if (shouldSpawnSmokescreen && smokescreenPrefab != null)
        {
            SpawnSmokescreen();
        }
        base.Die();
    }

    private void SpawnSmokescreen()
    {
        Vector3 spawnPosition = transform.position + smokescreenOffset;
        GameObject smokescreen = Instantiate(smokescreenPrefab, spawnPosition, Quaternion.identity);

        if (smokescreenDuration > 0)
        {
            Destroy(smokescreen, smokescreenDuration);
        }
    }

    public void SetSmokescreenPrefab(GameObject prefab)
    {
        smokescreenPrefab = prefab;
    }
}

