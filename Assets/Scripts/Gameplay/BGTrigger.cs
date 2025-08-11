using UnityEngine;

public class BGTrigger : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public float backgroundWidth = 65f;

    private bool hasSpawnedLeft = false;
    private bool hasSpawnedRight = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 parentPos = transform.parent.position;

        // Spawn to the right
        if (!hasSpawnedRight && other.transform.position.x > parentPos.x)
        {
            Vector3 spawnPosition = parentPos + new Vector3(backgroundWidth, 0f, 0f);
            Instantiate(backgroundPrefab, spawnPosition, Quaternion.identity);
            hasSpawnedRight = true;
        }

        // Spawn to the left
        if (!hasSpawnedLeft && other.transform.position.x < parentPos.x)
        {
            Vector3 spawnPosition = parentPos - new Vector3(backgroundWidth, 0f, 0f);
            Instantiate(backgroundPrefab, spawnPosition, Quaternion.identity);
            hasSpawnedLeft = true;
        }
    }
}