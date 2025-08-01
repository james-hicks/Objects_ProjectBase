using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float damage;

    private string targetTag;

    public void SetBullet(float _damage, string _target, float _speed = 10)
    {
        this.speed = _speed;
        this.damage = _damage;
        this.targetTag = _target;
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void Damage(IDamageable damageable)
    {
        damageable.GetDamage(damage);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(targetTag)) 
            return;

        if(collision.TryGetComponent(out IDamageable damageable))
        {
            Damage(damageable);
        }
    }
}
