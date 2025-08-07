using UnityEngine;

public class ShieldPickup : Pickup
{
    [SerializeField] private float shieldDuration = 5f;

    public override void OnPickup()
    {
        Player player = FindFirstObjectByType<Player>();
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        //if (player != null && playerInput != null)
        //{
        //    playerInput.ActivateShield(shieldDuration);
        //}

        base.OnPickup();
    }
}
