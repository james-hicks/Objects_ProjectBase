using System;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayableObject
{
    [SerializeField] private Camera cam;
    [SerializeField] public float speed;

    [Header("Default Weapon Info")]
    [SerializeField] private float weaponDamage = 1f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Bullet bulletPrefab;


    public Action<float, float> OnHealthSet;
    public Action<float> OnHealthUpdate;

    public Action OnDeath;

    //[Space]
    //[Header("UI")]

    private Rigidbody2D rb;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        health = new Health(100, 0.5f, 50);
        OnHealthSet?.Invoke(health.GetMaxHealth(), health.GetCurrentHealth());

        rb = GetComponent<Rigidbody2D>();

        weapon = new Weapon("PlayerWeapon", weaponDamage, bulletSpeed);
    }

    public override void Attack(float interval)
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        Debug.Log("Player Died!");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public override void Move(Vector2 direction, Vector2 target)
    {
        // Move the player in the direction they need to move, based on their speed
        rb.linearVelocity = direction * speed * Time.deltaTime;

        // Get the players position relative to the center of the camera, aka cam.worldToScreenPoint
        var playerScreenPos = cam.WorldToScreenPoint(transform.position);
        target.x -= playerScreenPos.x;
        target.y -= playerScreenPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public override void Shoot()
    {
        weapon.Shoot(bulletPrefab, "Enemy", this);
    }

    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
        Debug.Log("Player took " + damage + " Damage! " + health.GetCurrentHealth());
        OnHealthUpdate?.Invoke(health.GetCurrentHealth());
    }


    private void Update()
    {
        health.RegenHealth();
        OnHealthUpdate?.Invoke(health.GetCurrentHealth());
    }
}

