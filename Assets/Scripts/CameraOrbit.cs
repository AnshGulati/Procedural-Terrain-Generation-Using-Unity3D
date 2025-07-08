using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;         // Player transform
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float rotationSpeed = 5f;

    private float yaw = 0f;

    void LateUpdate()
    {
        if (!target) return;

        // Mouse X controls horizontal orbit
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;

        // Rotate around the player
        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target);
    }
}
