using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private float minHealth;
    [SerializeField] private float maxHealth;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public override void OnPickup()
    {
        // Increase health
        float health = Random.Range(minHealth, maxHealth);

        Player player = GameManager.GetInstance().GetPlayer();

        player.health.AddHealth((int)health);

        base.OnPickup();
        audioManager.PlaySFX(audioManager.healCollect);
    }
}
