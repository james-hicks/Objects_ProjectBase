using UnityEngine;

public class MultiBulletPickup : Pickup
{
    [Header("Multi-Bullet Settings")]
    [SerializeField] private float duration = 20f;
    [SerializeField] private int maxStacks = 5;

    
    public override void OnPickup()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.ActivateMultiBullet(duration, maxStacks);  
            }
        }

        base.OnPickup();
    }    
}
