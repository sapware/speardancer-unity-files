
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;

    public float jumpForce = 10f;
    public float boostForce = 12f;

    public GameObject spear;
    public GameObject droppedSpearPrefab;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;

    private bool isGrounded = false;
    private bool boostAvailable = true;
    private bool isBoosting = false;
    private bool spearEquipped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spear != null)
        {
            spear.SetActive(false);
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Detect actual ground beneath the player's feet
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        // Reset spear boost when grounded
        if (isGrounded)
        {
            boostAvailable = true;
        }

        // Equip / Unequip Spear
        if (Input.GetKeyDown(KeyCode.E))
        {
            spearEquipped = !spearEquipped;

            if (spear != null)
            {
                spear.SetActive(spearEquipped);
            }
        }

        // Jump ONLY when actually grounded
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }

        // Spear Boost
        if (spearEquipped &&
            Input.GetKeyDown(KeyCode.Q) &&
            !isGrounded &&
            boostAvailable &&
            !isBoosting)
        {
            StartCoroutine(SpearBoost());
        }
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        boostAvailable = false;

        // Remove spear from hand
        spearEquipped = false;

        if (spear != null)
        {
            spear.SetActive(false);
        }

        // Spawn spear beneath player at 90 degrees
        if (droppedSpearPrefab != null)
        {
            Instantiate(
                droppedSpearPrefab,
                new Vector3(
                    transform.position.x,
                    transform.position.y - 1f,
                    0f
                ),
                Quaternion.Euler(0f, 0f, 90f)
            );
        }

        // Brief hover
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.35f);

        // Launch upward
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            boostForce
        );

        isBoosting = false;
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        // Crouch
        if (Input.GetKey(KeyCode.C))
        {
            currentSpeed = crouchSpeed;
        }

        // Horizontal movement
        if (!isBoosting)
        {
            rb.linearVelocity = new Vector2(
                moveInput * currentSpeed,
                rb.linearVelocity.y
            );
        }
    }

    // Visualise the ground detection area in the Unity Editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}
