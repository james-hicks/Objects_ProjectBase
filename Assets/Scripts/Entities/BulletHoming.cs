using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class BulletHoming : Bullet
{
    public Transform player;
    protected Rigidbody2D rb;
    private const string PLAYER_TAG = "Player";

    public float attackrange;


    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();

        
    }

    protected void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
        else
        {
            // Try to find player again if we don't have one
            GameObject playerGO = GameObject.FindGameObjectWithTag(PLAYER_TAG);
            if (playerGO != null)
            {
                player = playerGO.transform;
            }
        }
    }
        

    private void FollowPlayer()
    {
        if (player == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag(PLAYER_TAG);
            if (playerGO != null)
            {
                player = playerGO.transform;
            }
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackrange)
        {
            // Move towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

}
