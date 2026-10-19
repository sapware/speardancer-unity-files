using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;
    public float jumpForce = 5f;
    public float boostForce = 15f;

    private Rigidbody2D rb;
    private float moveInput;

    private bool isGrounded = true;
    private bool boostAvailable = true;
    private bool isBoosting = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Normal jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // Mid-air boost
        if (Input.GetKeyDown(KeyCode.Q) && !isGrounded && boostAvailable && !isBoosting)
        {
            StartCoroutine(BoostJump());
        }
    }

    IEnumerator BoostJump()
    {
        isBoosting = true;
        boostAvailable = false;

        // Freeze in air
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        // Restore gravity
        rb.gravityScale = 3;

        // Launch upward
        rb.linearVelocity = new Vector2(0, boostForce);

        isBoosting = false;
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.C))
        {
            currentSpeed = crouchSpeed;
        }

        if (!isBoosting)
        {
            rb.linearVelocity = new Vector2(
                moveInput * currentSpeed,
                rb.linearVelocity.y
            );
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        boostAvailable = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}