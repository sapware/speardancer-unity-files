
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;
    public float jumpForce = 10f;
    public float boostForce = 12f;

    [Header("Character Sprites")]
    public GameObject standingSprite;
    public GameObject crouchingSprite;

    [Header("Spear")]
    public GameObject spear;
    public GameObject droppedSpearPrefab;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Crouch Hitbox")]
    public float crouchHeight = 0.8f;

    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;

    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    private float moveInput;

    private bool isGrounded = false;
    private bool boostAvailable = true;
    private bool isBoosting = false;
    private bool spearEquipped = false;
    private bool isCrouching = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();

        if (rb == null)
            Debug.LogError("Player is missing Rigidbody2D!");

        if (playerCollider != null)
        {
            standingColliderSize = playerCollider.size;
            standingColliderOffset = playerCollider.offset;
        }
        else
        {
            Debug.LogError(
                "Player needs a CapsuleCollider2D for crouching!"
            );
        }

        if (standingSprite != null)
            standingSprite.SetActive(true);

        if (crouchingSprite != null)
            crouchingSprite.SetActive(false);

        if (spear != null)
            spear.SetActive(false);
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Ground detection
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        if (isGrounded)
            boostAvailable = true;

        // Crouch while holding C
        isCrouching = Input.GetKey(KeyCode.C);

        // Change character appearance
        if (standingSprite != null)
            standingSprite.SetActive(!isCrouching);

        if (crouchingSprite != null)
            crouchingSprite.SetActive(isCrouching);

        // Change hitbox
        UpdateCrouchHitbox();

        // Equip / unequip spear
        if (Input.GetKeyDown(KeyCode.E))
        {
            spearEquipped = !spearEquipped;

            if (spear != null)
                spear.SetActive(spearEquipped);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }

        // Spear boost
        if (spearEquipped &&
            Input.GetKeyDown(KeyCode.Q) &&
            !isGrounded &&
            boostAvailable &&
            !isBoosting)
        {
            StartCoroutine(SpearBoost());
        }
    }

    void UpdateCrouchHitbox()
    {
        if (playerCollider == null)
            return;

        if (isCrouching)
        {
            // Shorten the hitbox while keeping its bottom near the floor
            float newHeight = Mathf.Clamp(
                crouchHeight,
                0.1f,
                standingColliderSize.y
            );

            playerCollider.size = new Vector2(
                standingColliderSize.x,
                newHeight
            );

            playerCollider.offset = new Vector2(
                standingColliderOffset.x,
                standingColliderOffset.y -
                (standingColliderSize.y - newHeight) / 2f
            );
        }
        else
        {
            // Restore the original hitbox
            playerCollider.size = standingColliderSize;
            playerCollider.offset = standingColliderOffset;
        }
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        boostAvailable = false;

        spearEquipped = false;

        if (spear != null)
            spear.SetActive(false);

        // Spawn spear rotated 90 degrees
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
        if (rb == null)
            return;

        float currentSpeed = isCrouching
            ? crouchSpeed
            : moveSpeed;

        if (!isBoosting)
        {
            rb.linearVelocity = new Vector2(
                moveInput * currentSpeed,
                rb.linearVelocity.y
            );
        }
    }

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
