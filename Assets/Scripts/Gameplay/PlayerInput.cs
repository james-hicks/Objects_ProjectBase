using UnityEngine;

[RequireComponent(typeof(Player))] // Ensure a Player component exists on this GameObject
public class PlayerInput : MonoBehaviour
{
    private Player player;// Reference to Player in inspector
    private float horizontal, vertical; // Input axes
    private Vector2 lookTarget;  // Mouse pos

    void Start()
    {
        player = GetComponent<Player>(); // Get Player component
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal"); // Get horizontal input arrows
        vertical = Input.GetAxis("Vertical"); // Get vertical input arrows
        lookTarget = Input.mousePosition; // Get mouse pos

        if (Input.GetMouseButtonDown(0))  // If  mouse  pressed
        {
            if (!player.IsRapidFireActive) // only if it is not power up time
            {
                player.Shoot();   // Player shoots
            }  
        }

        // Logic to activate Nuke with "space"
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //find inventory
            var inventory = player.GetComponent<InventoryManager>();
            if (inventory != null && inventory.ConsumeNuke())
            {
                NukePickup.ActivateNuke(); // activate nuke
            }
        }
    }

    private void FixedUpdate()
    {
        player.Move(new Vector2(horizontal, vertical), lookTarget); // Move player with input and mouse pos
    }
}
