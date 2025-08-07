using UnityEngine;

public class SmokescreenController : MonoBehaviour
{
    public float slowdownPercentage = 0.5f;
    private Player player;
    private bool playerInside = false;

    
    private static int stunAreaCount = 0;
    private static float originalPlayerSpeed;
    private static bool isPlayerStunned = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !playerInside)
        {
            player = other.GetComponent<Player>();
            playerInside = true;

            if (player != null)
            {
                stunAreaCount++;

               
                if (stunAreaCount == 1 && !isPlayerStunned)
                {
                    originalPlayerSpeed = player.speed;
                    player.speedMulti = slowdownPercentage;
                    isPlayerStunned = true;                    
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && playerInside && player != null)
        {
            playerInside = false;
            stunAreaCount--;

            
            if (stunAreaCount <= 0 && isPlayerStunned)
            {
                player.speed = originalPlayerSpeed;
                player.speedMulti = 1f;
                isPlayerStunned = false;
                stunAreaCount = 0; 
            }
        }
    }

    
    void OnDestroy()
    {
        if (playerInside && player != null)
        {
            stunAreaCount--;
            if (stunAreaCount <= 0)
            {
                stunAreaCount = 0;
                isPlayerStunned = false;
            }
        }
    }
}
