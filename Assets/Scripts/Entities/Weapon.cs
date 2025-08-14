using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        AudioManager.Instance.PlaySound("shoot");
        GameObject.Destroy(tempBullet.gameObject, _timeToDie);
    }

    // scatter shoot weapon to shoot in a given direction
    public void Shoot(Bullet _bullet, string _targetTag, PlayableObject _object, Quaternion rotation, float _timeToDie = 5)
    {
        Bullet tempBullet = GameObject.Instantiate(_bullet, _object.transform.position, rotation);
        tempBullet.SetBullet(damage, _targetTag, bulletSpeed);
        AudioManager.Instance.PlaySound("shoot");
        GameObject.Destroy(tempBullet.gameObject, _timeToDie);
    }

    public float GetDamage()
    {
        return damage;
    }
}
