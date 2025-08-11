using UnityEngine;

public class BGScroll : MonoBehaviour
{
    public Transform player; // get player, as background will slide with him
    public float slideSpeed = 0.1f; // 0 = static, 1 = full follow, 0.1f for mountains = slow

    private Vector3 lastPlayerPosition; 

    // Start is called before the first frame update
    void Start()
    {
        // error handling: if no player assigned in unity, gets player by tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        lastPlayerPosition = player.position; // initialize position
    }

    // Update is called once per frame
    void Update()
    {
        // calculate how far the player moved since the last frame
        Vector3 deltaMovement = player.position - lastPlayerPosition;
        // move the background horizontally and vertically at reduced speed
        transform.position += new Vector3(deltaMovement.x * slideSpeed, deltaMovement.y * slideSpeed, 0);
        // update player position
        lastPlayerPosition = player.position;
    }
}
