using UnityEngine;

public class PickupNuke : Pickup
{
    public override void OnPickup()
    {
        Player player = FindFirstObjectByType<Player>();
        
        base.OnPickup();
    }


    // targeting signaling death
    public static void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}


