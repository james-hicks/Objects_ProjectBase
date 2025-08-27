using UnityEngine;

// Gets picked up by player and added to inventory.
// UI shows current inventory.
// When activated with space, destroys all enemies, pickups, and bullets in the scene.

public class NukePickup : Pickup
{

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }


    public override void OnPickup()
    {
        // Find the player's Inventory component
        InventoryManager inv = GameObject.FindWithTag("Player").GetComponent<InventoryManager>();

        // If inventory exists, add this nuke to it
        if (inv != null)
        {
            inv.AddNuke();
            audioManager.PlaySFX(audioManager.nukeCollect);
            
        }

        // Call base, destroy object
        base.OnPickup();
    }

    // Static method to activate the nuke effect
    public static void ActivateNuke()
    {
        
        // Destroy all enemies in the scene
        foreach (Enemy item in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Destroy(item.gameObject);
        }

        // Destroy all bullets in the scene
        foreach (Bullet item in FindObjectsByType<Bullet>(FindObjectsSortMode.None))
        {
            Destroy(item.gameObject);
        }

        // Destroy all pickups in the scene
        foreach (Pickup item in FindObjectsByType<Pickup>(FindObjectsSortMode.None))
        {
            Destroy(item.gameObject);
        }
        
    }
}
