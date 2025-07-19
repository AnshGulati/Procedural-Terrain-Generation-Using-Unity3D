using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ManualJump : MonoBehaviour
{
    public float jumpForce = 5f;
    public float gravity = -15f;

    private CharacterController controller;
    private float verticalVelocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Stick to the ground
        }

        // Jump when J is pressed
        if (isGrounded && Input.GetKeyDown(KeyCode.J))
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Apply movement
        Vector3 move = new Vector3(0, verticalVelocity, 0);
        controller.Move(move * Time.deltaTime);
    }
}