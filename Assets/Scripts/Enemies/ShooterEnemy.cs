using UnityEngine;
using System.Collections;

public class ShooterEnemy : Enemy
{
    [Header("Shooter Settings")]
    [SerializeField] private int baseHealth = 2; // Starting health
    [SerializeField] private float targetDistance = 5f; // Distance from player
    private bool isRetreating = false; // Tracks if moving away from player
    public float fireRate = 3f; // seconds before he can shoot again

    [Header("Default Weapon Info")]
    [SerializeField] private float weaponDamage = 1f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] protected Bullet bulletPrefab;
    private Coroutine shootingCoroutine;

    protected override void Start()
    {
        base.Start();
        health = new Health(baseHealth, 0); // Init health
        weapon = new Weapon("ShooterWeapon", weaponDamage, bulletSpeed); // init weapon
        shootingCoroutine = StartCoroutine(ShootRoutine()); // Start shooting loop
    }

    protected override void Update()
    {
        if (target == null || target.Equals(null)) return; // Exit if no player

        float distance = Vector2.Distance(transform.position, target.position);

        // Move toward player if too far and was retreating
        if (distance > targetDistance && isRetreating)
        {
            speed = Mathf.Abs(speed);
            isRetreating = false;
            base.Update();
        }

        // Move away if too close and wasn't already retreating
        if (distance < targetDistance && !isRetreating)
        {
            speed = -Mathf.Abs(speed);
            isRetreating = true;
            base.Update();
        }
    }

    // Coroutine that repeatedly heals allies at intervals
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }
        
    public override void Shoot()
    {
        if (weapon != null)
        {
            // Call weapon shoot with "Player" as targetTag
            weapon.Shoot(bulletPrefab, "Player", this);
            //Debug.Log("ShooterEnemy is shooting");
        }
    }
}
