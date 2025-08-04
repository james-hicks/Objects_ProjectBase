using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickup();
        }
    }

    public virtual void OnPickup()
    {
        Destroy(gameObject);
    }
}
