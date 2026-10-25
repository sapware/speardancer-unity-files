
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f;
    public float boostForce = 12f;

    [Header("Player Appearance")]
    public Sprite playerSprite;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Spear")]
    public GameObject spearInHand;
    public GameObject spearPrefab;
    public float spearThrowSpeed = 15f;

    [Header("Spear Grapple")]
    public float pullSpeed = 12f;
    public float pullStopDistance = 0.8f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool spearEquipped = true;
    private bool isBoosting;
    private bool hasBoostedThisJump;
    private bool isPulling;

    private SpearProjectile activeThrownSpear;
    private KeyCode grappleKey;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("Player needs a Rigidbody2D!");

        if (spearInHand != null)
            spearInHand.SetActive(true);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null && playerSprite != null)
        {
            sr.sprite = playerSprite;
        }
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

        // Equip or unequip spear.
        if (Input.GetKeyDown(KeyCode.E))
            ToggleSpear();

        // Throw to the right with X.
        if (Input.GetKeyDown(KeyCode.X)
            && spearEquipped
            && !isBoosting)
        {
            ThrowSpear(Vector2.right, KeyCode.X);
        }

        // Throw to the left with Z.
        if (Input.GetKeyDown(KeyCode.Z)
            && spearEquipped
            && !isBoosting)
        {
            ThrowSpear(Vector2.left, KeyCode.Z);
        }

        // Grapple only while holding the matching throw key.
        isPulling =
            activeThrownSpear != null
            && activeThrownSpear.IsStuck
            && Input.GetKey(grappleKey)
            && !isBoosting;

        // One spear boost per jump with Q.
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

        // Pull towards the stuck spear.
        if (isPulling && activeThrownSpear != null)
        {
            Vector2 targetPosition =
                activeThrownSpear.transform.position;

            Vector2 toSpear = targetPosition - rb.position;

            if (toSpear.magnitude <= pullStopDistance)
            {
                rb.linearVelocity = Vector2.zero;
                isPulling = false;
                return;
            }

            rb.linearVelocity = toSpear.normalized * pullSpeed;
            return;
        }

        // Normal horizontal movement.
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

    void ThrowSpear(Vector2 direction, KeyCode pullKey)
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
            Quaternion.identity
        );

        SpearProjectile projectile =
            thrownSpear.GetComponent<SpearProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "The spear prefab needs a SpearProjectile component!"
            );

            Destroy(thrownSpear);
            return;
        }

        projectile.Launch(direction, spearThrowSpeed);

        activeThrownSpear = projectile;
        grappleKey = pullKey;

        // Hide the held spear after throwing.
        spearEquipped = false;

        if (spearInHand != null)
            spearInHand.SetActive(false);
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        isPulling = false;
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
