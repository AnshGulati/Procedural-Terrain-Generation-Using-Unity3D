using UnityEngine;

public class CameraOrbitt : MonoBehaviour
{
    public Transform player;                  // Assign your player here
    public float distance = 4.0f;             // Distance from the player
    public float height = 1.5f;               // Height above the player
    public float rotationSpeed = 3.0f;        // Mouse sensitivity

    private float yaw = 0.0f;                 // Horizontal rotation
    private float pitch = 20.0f;              // Vertical rotation
    public float pitchMin = -20f, pitchMax = 60f;

    void Start()
    {
        if (player != null)
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Get mouse input
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Convert angles to position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        offset.y += height;

        // Set position and look at player
        transform.position = player.position + offset;
        transform.LookAt(player.position + Vector3.up * height);
    }
}

