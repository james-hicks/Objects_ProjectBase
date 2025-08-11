using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayableObject
{
    [SerializeField] private Camera cam; // Reference to main camera for aiming
    [SerializeField] private float speed = 5f;  // Move speed

    [Header("Default Weapon Info")]
    [SerializeField] private float weaponDamage = 1f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Bullet bulletPrefab;

    public Action<float, float> OnHealthSet;
    public Action<float> OnHealthUpdate;
    public Action OnDeath;

    // variables for rapid fire coroutine
    private Coroutine rapidFireRoutine;
    private bool rapidFireActive;
    private float rapidFireRate;
    private float minY = -4f; // Simple Y pos limit, player cannot go below this

    public bool IsRapidFireActive => rapidFireActive;



    [Space]
    [Header("UI")]
    [SerializeField] private Slider healthBar;
    private Rigidbody2D rb;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        health = new Health(100, 0.5f, 70); // Initialize health (max HP, regen rate, start HP)
        OnHealthSet?.Invoke(health.GetMaxHealth(), health.GetCurrentHealth());
        //healthBar.maxValue = health.GetMaxHealth();
        rb = GetComponent<Rigidbody2D>();

        weapon = new Weapon("PlayerWeapon", weaponDamage, bulletSpeed); //initialize weapon
    }



    public override void Move(Vector2 direction, Vector2 target)
    {
        // Move with keys (currently using placeholder direction)
        rb.linearVelocity = direction * speed * Time.deltaTime;

        // Calculate screen position for aiming
        var playerScreenPos = cam.WorldToScreenPoint(transform.position);
        target.x -= playerScreenPos.x;
        target.y -= playerScreenPos.y;

        // Rotate to face mouse cursor
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // shoot normally
    public override void Shoot()
    {
        if (weapon != null)
        {
            // Call weapon shoot with "Enemy" as targetTag
            weapon.Shoot(bulletPrefab, "Enemy", this);
        }
    }

    // Rapid fire during powerup time
    public void StartRapidFire(float rate, float duration)
    {
        if (rapidFireRoutine != null) StopCoroutine(rapidFireRoutine);
        rapidFireRoutine = StartCoroutine(RapidFire(duration, rate));
    }

    // rapid fire coroutine
    private IEnumerator RapidFire(float duration, float rate)
    {
        Debug.Log($"RapidFire coroutine started with duration: {duration}, rate: {rate}");


        rapidFireActive = true;
        rapidFireRate = rate;

        float elapsed = 0f;
        float nextShotAt = 0f;

        while (elapsed < duration)
        {
            // Only fire while the mouse is held
            if (Input.GetMouseButton(0) && Time.time >= nextShotAt)
            {
                Shoot();
                nextShotAt = Time.time + 1f / rapidFireRate;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rapidFireActive = false;
        Debug.Log("Rapid fire ended");
        rapidFireRoutine = null;
    }

    // need to keep to avoid compiler errors
    public override void Attack(float interval) { }

    public override void Die()
    {
        Debug.Log("Player has died! StackTrace:" + Environment.StackTrace); // Log death
        OnDeath?.Invoke(); // Call player's death
        Destroy(gameObject); // Destroy player
    }

    // Take damage implementation
    public override void GetDamage(float damage)
    {
        base.GetDamage(damage); // Call base to reduce health and check death
        //Debug.Log("Player took " + damage + " damage. " + health.GetCurrentHealth());
        OnHealthUpdate?.Invoke(health.GetCurrentHealth());
    }

    private void Update()
    {
        health.RegenHealth();
        OnHealthUpdate?.Invoke(health.GetCurrentHealth());

        // Clamp the player's Y position
        Vector3 pos = transform.position;
        if (pos.y < minY)
            pos.y = minY;
        transform.position = pos;
    }
}
