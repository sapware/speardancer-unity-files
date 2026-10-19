using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private float moveInput;

    private int jumpsRemaining = 2;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Jump and Double Jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        // Hold C to crouch
        if (Input.GetKey(KeyCode.C))
        {
            currentSpeed = crouchSpeed;
        }

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        jumpsRemaining = 2;
    }
}