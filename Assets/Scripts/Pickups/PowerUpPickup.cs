using UnityEngine;

public class PowerUpPickup : Pickup
{
    [SerializeField] private float duration = 5f;  // buff duration
    [SerializeField] private float rapidFireRate = 10f; // shots per second while powered

    public override void OnPickup()
    {
        // get player from scene
        Player player = GameManager.GetInstance().GetPlayer();
        // start rapid fire
        player.StartRapidFire(rapidFireRate, duration);

        // find fireUI part of player prefab
        RapidFireUI fireUI = player.GetComponentInChildren<RapidFireUI>(true);
        // error checking
        if (fireUI != null)
        {
            fireUI.StartTimer(duration); // start rapid fire duration
        }
        else
        {
            Debug.LogWarning("RapidFireUI not found on player.");
        }

        base.OnPickup(); // destroy this pickup
    }
}
