using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    Rigidbody rb;
    Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(h, 0f, v).normalized;
    }

    void FixedUpdate()
    {
        Vector3 move = inputDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
