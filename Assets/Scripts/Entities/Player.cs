using System;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayableObject
{
    [SerializeField] private Camera cam;
    [SerializeField] public float speed;
    [SerializeField] public float speedMulti = 1;

    [Header("Default Weapon Info")]
    [SerializeField] private float weaponDamage = 1f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Bullet bulletPrefab;


    [SerializeField] ParticleSystem onDeathExplosion;

    private ParticleSystem explosionInstance;
    private Vector3 lastValidPosition;

    public Action<float, float> OnHealthSet;
    public Action<float> OnHealthUpdate;

    public Action OnDeath;

    //[Space]
    //[Header("UI")]

    private Rigidbody2D rb;

    private void Awake()
    {
        //onDeathExplosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cam = Camera.main;
    }

    private void Start()
    {

        health = new Health(100, 0.5f, 50);
        OnHealthSet?.Invoke(health.GetMaxHealth(), health.GetCurrentHealth());

        rb = GetComponent<Rigidbody2D>();

        weapon = new Weapon("PlayerWeapon", weaponDamage, bulletSpeed);
        lastValidPosition = transform.position;
    }

    public override void Attack(float interval)
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {

        Debug.Log("Player Died!");

        //onDeathExplosion.Play();
        spawnParticles();
        OnDeath?.Invoke();

        Destroy(gameObject);
    }

    public override void Move(Vector2 direction, Vector2 target)
    {
        // Move the player in the direction they need to move, based on their speed
        rb.linearVelocity = direction * speed * speedMulti * Time.deltaTime;

        // Get the players position relative to the center of the camera, aka cam.worldToScreenPoint
        var playerScreenPos = cam.WorldToScreenPoint(transform.position);
        target.x -= playerScreenPos.x;
        target.y -= playerScreenPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);


    }

    public override void Shoot()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();

        if (playerInput != null && playerInput.IsMultiBulletActive())
        {
            // Multi-bullet shooting with angled bullets
            MultiBulletShoot(playerInput.GetMultiBulletStacks());
        }
        else
        {
            // Normal single bullet
            weapon.Shoot(bulletPrefab, "Enemy", this);
        }
    }

    public override void GetDamage(float damage)
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null && input.IsShieldActive())  // <- Use the public method instead
        {
            Debug.Log("Damage blocked by shield!");
            return; // Block all damage when shield is active
        }

        base.GetDamage(damage);
        Debug.Log("Player took " + damage + " Damage! " + health.GetCurrentHealth());

        OnHealthUpdate?.Invoke(health.GetCurrentHealth());
    }


    private void Update()
    {
        health.RegenHealth();

        OnHealthUpdate?.Invoke(health.GetCurrentHealth());

        lastValidPosition = transform.position;
    }

    //scatter shoot method to shoot in 8 directions
    public void scatterShoot()
    {
        Vector2[] direction = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(-1, -1).normalized
        };

        foreach (Vector2 dir in direction)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg);
            weapon.Shoot(bulletPrefab, "Enemy", this, rotation);
        }
    }

    private void spawnParticles()
    {
        explosionInstance = Instantiate(onDeathExplosion, transform.position, quaternion.identity);
        Destroy(explosionInstance.gameObject, 3f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Map Boundry")
        {
            // Player is leaving the map area
            rb.linearVelocity = Vector2.zero;
            transform.position = lastValidPosition;
            Debug.Log("Player returned to boundary");
        }
    }
    private void MultiBulletShoot(int bulletCount)
    {
        // Shoot the main bullet first
        weapon.Shoot(bulletPrefab, "Enemy", this);

        // Shoot additional bullets with clear angle separation
        float baseAngle = 20f; // Base angle separation

        for (int i = 1; i <= bulletCount; i++)
        {
            // Alternate between positive and negative angles
            float angle = (i % 2 == 1) ? baseAngle * ((i + 1) / 2) : -baseAngle * (i / 2);

            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, angle);
            weapon.Shoot(bulletPrefab, "Enemy", this, rotation);
        }

        Debug.Log($"Multi-bullet fired: {bulletCount + 1} bullets total");
    }

    
}

