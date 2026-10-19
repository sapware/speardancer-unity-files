
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;
    public float jumpForce = 10f;
    public float boostForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Character Sprites")]
    public GameObject standingSprite;
    public GameObject crouchingSprite;

    [Header("Crouching Hitboxes")]
    public BoxCollider2D standingHitbox;
    public BoxCollider2D crouchingHitbox;

    [Header("Spear")]
    public GameObject spearInHand;
    public GameObject spearPrefab;
    public Transform spearSpawnPoint;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCrouching;
    private bool spearEquipped = true;
    private bool isBoosting;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Player needs a Rigidbody2D!");
        }

        // Start with the standing character and hitbox.
        isCrouching = false;

        if (standingSprite != null)
            standingSprite.SetActive(true);

        if (crouchingSprite != null)
            crouchingSprite.SetActive(false);

        if (standingHitbox != null)
            standingHitbox.enabled = true;

        if (crouchingHitbox != null)
            crouchingHitbox.enabled = false;
    }

    void Update()
    {
        // Check whether the player is touching the ground.
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        // Crouching.
        isCrouching = Input.GetKey(KeyCode.C);

        UpdateCrouchingSprites();
        UpdateCrouchingHitbox();

        // Jump.
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Equip or unequip spear.
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleSpear();
        }

        // Spear boost while airborne.
        if (Input.GetKeyDown(KeyCode.Q) && !isGrounded && spearEquipped && !isBoosting)
        {
            StartCoroutine(SpearBoost());
        }
    }

    void FixedUpdate()
    {
        if (isBoosting)
            return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        rb.linearVelocity = new Vector2(
            horizontalInput * currentSpeed,
            rb.linearVelocity.y
        );
    }

    void UpdateCrouchingSprites()
    {
        if (standingSprite != null)
            standingSprite.SetActive(!isCrouching);

        if (crouchingSprite != null)
            crouchingSprite.SetActive(isCrouching);
    }

    void UpdateCrouchingHitbox()
    {
        if (standingHitbox != null)
            standingHitbox.enabled = !isCrouching;

        if (crouchingHitbox != null)
            crouchingHitbox.enabled = isCrouching;
    }

    void ToggleSpear()
    {
        spearEquipped = !spearEquipped;

        if (spearInHand != null)
            spearInHand.SetActive(spearEquipped);
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        spearEquipped = false;

        if (spearInHand != null)
            spearInHand.SetActive(false);

        // Spawn the dropped spear.
        if (spearPrefab != null)
        {
            Vector3 spawnPosition = spearSpawnPoint != null
                ? spearSpawnPoint.position
                : transform.position + Vector3.down;

            GameObject droppedSpear = Instantiate(
                spearPrefab,
                spawnPosition,
                Quaternion.Euler(0f, 0f, 90f)
            );

            // Let the spear hover briefly.
            Rigidbody2D spearRb = droppedSpear.GetComponent<Rigidbody2D>();

            if (spearRb != null)
                spearRb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(0.35f);

            // Launch the player upwards.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, boostForce);
        }
        else
        {
            Debug.LogWarning("Spear Prefab has not been assigned!");
            yield return new WaitForSeconds(0.35f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, boostForce);
        }

        isBoosting = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
