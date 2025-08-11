using UnityEngine;

// This script makes the camera follow the player smoothly
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // The player to follow
    [SerializeField] private float smoothSpeed = 0.125f; // Smoothing factor

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}