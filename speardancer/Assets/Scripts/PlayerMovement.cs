
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f;
    public float boostForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Spear")]
    public GameObject spearInHand;
    public GameObject spearPrefab;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool spearEquipped = true;
    private bool isBoosting;
    private bool hasBoostedThisJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Player needs a Rigidbody2D!");
        }
    }

    void Update()
    {
        if (rb == null)
            return;

        // Check whether the player is touching the ground.
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        // Allow another spear boost after landing.
        if (isGrounded)
        {
            hasBoostedThisJump = false;
        }

        // Jump using Up Arrow.
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        // Equip or unequip spear using E.
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleSpear();
        }

        // One spear boost per jump using Q.
        if (Input.GetKeyDown(KeyCode.Q)
            && !isGrounded
            && spearEquipped
            && !isBoosting
            && !hasBoostedThisJump)
        {
            hasBoostedThisJump = true;
            StartCoroutine(SpearBoost());
        }
    }

    void FixedUpdate()
    {
        if (rb == null || isBoosting)
            return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    void ToggleSpear()
    {
        spearEquipped = !spearEquipped;

        if (spearInHand != null)
        {
            spearInHand.SetActive(spearEquipped);
        }
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        spearEquipped = false;

        if (spearInHand != null)
        {
            spearInHand.SetActive(false);
        }

        // Spawn the dropped spear below the player.
        if (spearPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.down;

            GameObject droppedSpear = Instantiate(
                spearPrefab,
                spawnPosition,
                Quaternion.Euler(0f, 0f, 90f)
            );

            // Keep the spear stationary during the hover.
            Rigidbody2D spearRb =
                droppedSpear.GetComponent<Rigidbody2D>();

            if (spearRb != null)
            {
                spearRb.linearVelocity = Vector2.zero;
                spearRb.angularVelocity = 0f;
                spearRb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else
        {
            Debug.LogWarning("Spear Prefab has not been assigned!");
        }

        // Wait before launching the player.
        yield return new WaitForSeconds(0.35f);

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            boostForce
        );

        isBoosting = false;
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
