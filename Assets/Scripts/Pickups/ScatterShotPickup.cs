using UnityEngine;

public class ScatterShotPickup : Pickup
{
    [SerializeField] private float shotDuration = 5f;

    public override void OnPickup()
    {
        Player player = FindFirstObjectByType<Player>();
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (player != null && playerInput != null)
        {
            playerInput.ActivateScatterShot(shotDuration);
        }

        base.OnPickup();
    }
}
