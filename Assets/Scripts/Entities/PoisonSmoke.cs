using UnityEngine;

public class PoisonSmoke : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 3f;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float damageRadius = 3f;

    private Player playerInRange;
    private float lastDamageTime;
    private CircleCollider2D damageArea;

    void Start()
    {
        SetupDamageArea();
    }

    void SetupDamageArea()
    {
        damageArea = GetComponent<CircleCollider2D>();
        if (damageArea == null)
        {
            damageArea = gameObject.AddComponent<CircleCollider2D>();
        }

        damageArea.radius = damageRadius;
        damageArea.isTrigger = true;
    }

    void Update()
    {
        if (playerInRange != null && Time.time >= lastDamageTime + damageInterval)
        {
            ApplyDamageToPlayer();
            lastDamageTime = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = other.GetComponent<Player>();
            if (playerInRange != null)
            {
                Debug.Log("Player entered poison smoke!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = null;
            Debug.Log("Player left poison smoke!");
        }
    }

    void ApplyDamageToPlayer()
    {
        if (playerInRange != null)
        {
            float damageThisTick = damagePerSecond * damageInterval;
            playerInRange.GetDamage(damageThisTick);
        }
    }







}