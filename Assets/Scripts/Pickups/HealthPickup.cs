using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private float minHealth;
    [SerializeField] private float maxHealth;

    public override void OnPickup()
    {
        // Increase health
        float health = Random.Range(minHealth, maxHealth);

        Player player = GameManager.GetInstance().GetPlayer();

        player.health.AddHealth((int)health);

        base.OnPickup();
    }
}
