using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate the desired position[cite: 2]
        Vector3 desiredPosition = player.position + offset;

        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
