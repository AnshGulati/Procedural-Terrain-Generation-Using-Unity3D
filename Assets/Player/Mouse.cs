using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;

    private Transform parent;

    void Start()
    {
        parent = transform.parent;
        Cursor.lockState = CursorLockMode.Locked; // Hide & lock cursor
    }

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        parent.Rotate(Vector3.up, mouseX);
    }
}
