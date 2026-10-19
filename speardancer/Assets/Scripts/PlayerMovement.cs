
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
    public float spearThrowSpeed = 15f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool spearEquipped = true;
    private bool isBoosting;
    private bool hasBoostedThisJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("Player needs a Rigidbody2D!");

        if (spearInHand != null)
            spearInHand.SetActive(true);
    }

    void Update()
    {
        if (rb == null)
            return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        if (isGrounded)
            hasBoostedThisJump = false;

        // Jump.
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        // Equip or unequip.
        if (Input.GetKeyDown(KeyCode.E))
            ToggleSpear();

        // Debug and throw spear using R.
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key detected!");
            Debug.Log("Spear equipped: " + spearEquipped);
            Debug.Log("Is boosting: " + isBoosting);
            Debug.Log(
                "Spear prefab assigned: " + (spearPrefab != null)
            );

            if (spearEquipped && !isBoosting)
                ThrowSpear(Vector2.right);
        }

        // One spear boost per jump.
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
            spearInHand.SetActive(spearEquipped);
    }

    void ThrowSpear(Vector2 direction)
    {
        if (spearPrefab == null)
        {
            Debug.LogWarning("Assign the Spear Prefab!");
            return;
        }

        Vector3 spawnPosition =
            transform.position + (Vector3)(direction * 0.8f);

        GameObject thrownSpear = Instantiate(
            spearPrefab,
            spawnPosition,
            Quaternion.Euler(
                0f,
                0f,
                direction.x > 0 ? 90f : -90f
            )
        );

        SpearProjectile projectile =
            thrownSpear.GetComponent<SpearProjectile>();

        if (projectile != null)
        {
            projectile.Launch(direction, spearThrowSpeed);
        }
        else
        {
            Debug.LogError(
                "Your Spear Prefab needs a SpearProjectile component!"
            );
        }

        // Hide the held spear after throwing.
        spearEquipped = false;

        if (spearInHand != null)
            spearInHand.SetActive(false);
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        spearEquipped = false;

        if (spearInHand != null)
            spearInHand.SetActive(false);

        if (spearPrefab != null)
        {
            Vector3 spawnPosition =
                transform.position + Vector3.down;

            GameObject droppedSpear = Instantiate(
                spearPrefab,
                spawnPosition,
                Quaternion.Euler(0f, 0f, 90f)
            );

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
            Debug.LogWarning("Assign the Spear Prefab!");
        }

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
