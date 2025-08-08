using UnityEngine;

public class MultiShooterEnemy : ShooterEnemy
{
    [Header("Multishoot Settings")]
    [SerializeField] private float bulletSpread;
    [SerializeField] private int bulletCount;

    public override void Shoot()
    {
        if (weapon != null)
        {
            // Call weapon shoot with "Player" as targetTag
            weapon.MultiShoot(bulletPrefab, "Player", this, 5, bulletCount, bulletSpread);
           
        }
    }
}
