using UnityEngine;

public class SmokescreenController : MonoBehaviour
{
    public float slowdownPercentage = 0.5f; 
    private Player player;
    private float originalPlayerSpeed = 1f;
   
    private bool playerInside = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            player = other.GetComponent<Player>();
            if (player != null)
            {
                originalPlayerSpeed = player.speed;
                player.speed *= (1 - slowdownPercentage);
                playerInside = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && playerInside)
        {
            if (player != null)
            {
                player.speed = originalPlayerSpeed;
                playerInside = false;
            }
        }
    }
}