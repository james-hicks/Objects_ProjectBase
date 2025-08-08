using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Weapon
{
    private string name;
    private float damage;
    private float bulletSpeed;
 

    public Weapon(string _name, float _damage, float _bulletSpeed)
    {
        name = _name; 
        damage = _damage;
        bulletSpeed = _bulletSpeed;
    }

    public void Shoot(Bullet _bullet, string _targetTag, PlayableObject _object, float _timeToDie = 5)
    {
        Bullet tempBullet = GameObject.Instantiate(_bullet, _object.transform.position, _object.transform.rotation);
        tempBullet.SetBullet(damage, _targetTag, bulletSpeed);

        GameObject.Destroy(tempBullet.gameObject, _timeToDie);
    }

    public void MultiShoot(Bullet _bullet, string  _targetTag, PlayableObject _object, float _timeToDie, int _bulletCount, float _spreadAngle)
    {
        for (int I = 0; I < _bulletCount; I++)
        {
            float angleStep = (_bulletCount > 1) ? _spreadAngle / (_bulletCount - 1) : 0f;
            float angle = -_spreadAngle / 2f + (angleStep * I);

            Bullet tempBullet = GameObject.Instantiate(_bullet, _object.transform.position, _object.transform.rotation * Quaternion.Euler(0, 0, angle));
            tempBullet.SetBullet(damage, _targetTag, bulletSpeed);

            GameObject.Destroy(tempBullet.gameObject, _timeToDie);
        }  
    }

    public float GetDamage()
    {
        return damage;
    }
}
