using UnityEngine;
using System.Collections;

public class MachineGunEnemy : ShooterEnemy
{
    [Header("Machine Gunner Settings")]
    [SerializeField] private float fireRateOverride = 1f; // faster shooting rate than default
    [SerializeField] private int burstCount = 5; // number of bullets per burst
    [SerializeField] private float burstSpacing = 1f; // delay between bullets in a burst
    [SerializeField] private float weaponDamageOverride = 3f; // custom damage per bullet
    [SerializeField] private float bulletSpeedOverride = 3f; // custom bullet speed
    private bool isShooting = false;

    protected override void Start()
    {
        // override shoot cooldown with faster fire rate
        fireRate = fireRateOverride;

        base.Start(); // call parent start method (likely initializes timers, etc.)

        // initialize custom weapon for this enemy
        weapon = new Weapon("MachineGun", weaponDamageOverride, bulletSpeedOverride);
    }

    public override void Shoot()
    {
        if (!isShooting)
        {
            // override default shoot behavior with burst fire
            StartCoroutine(BurstFire());
        }
    }

    private IEnumerator BurstFire()
    {
        isShooting = true;

        for (int i = 0; i < burstCount; i++)
        {
            // instantiate and set up a bullet
            Bullet b = Instantiate(bulletPrefab, transform.position, transform.rotation);
            b.SetBullet(weaponDamageOverride, "Player", bulletSpeedOverride); // assign bullet damage, target, speed

            Destroy(b.gameObject, 5f); // destroy bullet after 5 seconds

            yield return new WaitForSeconds(burstSpacing); // delay before next shot in burst
        }

        yield return new WaitForSeconds(fireRate); // cooldown after the burst
        isShooting = false;
    }
}
