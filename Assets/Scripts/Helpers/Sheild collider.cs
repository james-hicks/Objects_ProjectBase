using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is a bullet
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Only destroy enemy bullets (bullets that target "Player")
            // Player bullets target "Enemy", so they should pass through
            if (bullet.IsEnemyBullet())
            {
                Debug.Log("Enemy bullet destroyed by shield!");
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("Player bullet passed through shield!");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle solid collider interactions for bullets
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Only destroy enemy bullets (bullets that target "Player")
            if (bullet.IsEnemyBullet())
            {
                Debug.Log("Enemy bullet destroyed by shield!");
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("Player bullet passed through shield!");
            }
        }
    }
}

