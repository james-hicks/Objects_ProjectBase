using UnityEngine;

public class PickupShot : Pickup
{
    [SerializeField] private float rapidFireDuration = 5f;
    [SerializeField] private float rapidFireRate = 0.1f; // Time between shots

    public override void OnPickup()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.ActivateRapidFire(rapidFireDuration, rapidFireRate);
            }
        }

        base.OnPickup(); 
    }
}
