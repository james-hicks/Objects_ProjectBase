using System.Collections;
using UnityEngine;

public class HealerEnemy : Enemy
{
    [Header("Healer Settings")]
    [SerializeField] private int baseHealth = 2; // Starting health
    [SerializeField] private float targetDistance = 5f; // Distance from player
    [SerializeField] private float healingInterval = 2f; // Time between heals
    [SerializeField] private float allyRadius = 3f; // Radius to search for allies
    [SerializeField] private float healAmount = 1f; // Amount to heal per ally
    private bool isRetreating = false; // Tracks if moving away from player
    private Coroutine healingCoroutine;

    protected override void Start()
    {
        base.Start();
        health = new Health(baseHealth, 0); // Initialize health
        healingCoroutine = StartCoroutine(HealRoutine()); // Start healing loop
    }

    protected override void Update()
    {
        if (target == null || target.Equals(null)) return; // Exit if no player

        float distance = Vector2.Distance(transform.position, target.position);

        // Move toward player if too far and was retreating
        if (distance > targetDistance && isRetreating)
        {
            speed = Mathf.Abs(speed);
            isRetreating = false;
            base.Update();
        }

        // Move away if too close and wasn't already retreating
        if (distance < targetDistance && !isRetreating)
        {
            speed = -Mathf.Abs(speed);
            isRetreating = true;
            base.Update();
        }
    }

    // Coroutine that repeatedly heals allies at intervals
    private IEnumerator HealRoutine()
    {
        while (true)
        {
            Heal();
            yield return new WaitForSeconds(healingInterval);
        }
    }

    // Heal nearby enemies (excluding self) within radius
    private void Heal()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, allyRadius);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue; // Skip self

            if (hit.CompareTag("Enemy")) // Only heal other enemies
            {
                if (hit.TryGetComponent<PlayableObject>(out var ally))
                {
                    ally.health.AddHealth(healAmount); // Heal ally
                    //Debug.Log($"Healed {hit.gameObject.name} by {healAmount}");
                }
            }
        }
    }
}
