using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    [Header("Area Effect Settings")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float duration = 3f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    public static void SpawnAreaEffect(GameObject prefab, Vector3 position, float duration = 3f, Vector3 offset = default)
    {
        if (prefab == null)
        {
            Debug.LogWarning("AreaEffect: No prefab assigned!");
            return;
        }

        Vector3 spawnPosition = position + offset;
        GameObject effectInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Auto-destroy after duration
        Destroy(effectInstance, duration);
    }

    public void TriggerAreaEffect()
    {
        SpawnAreaEffect(effectPrefab, transform.position, duration, spawnOffset);
    }

    public void TriggerAreaEffect(Vector3 customPosition)
    {
        SpawnAreaEffect(effectPrefab, customPosition, duration, spawnOffset);
    }

    public void TriggerAreaEffect(GameObject customPrefab, float customDuration)
    {
        SpawnAreaEffect(customPrefab, transform.position, customDuration, spawnOffset);
    }
}
