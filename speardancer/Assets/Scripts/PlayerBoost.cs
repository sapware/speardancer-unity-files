
using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(100)]
public class PlayerBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    public float boostSpeed = 18f;
    public float boostDuration = 0.2f;
    public float cooldown = 2f;

    [Header("Controls")]
    public KeyCode boostKey = KeyCode.LeftShift;

    private Rigidbody2D rb;

    private bool canBoost = true;
    private bool isBoosting = false;

    private int facingDirection = 1;
    private int boostDirection = 1;

    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            originalGravity = rb.gravityScale;
        }
        else
        {
            Debug.LogError(
                "PlayerBoost needs a Rigidbody2D on the same GameObject!"
            );
        }
    }

    void Update()
    {
        // Remember the last direction pressed.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            facingDirection = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            facingDirection = 1;
        }

        // Activate the boost with one button.
        if (Input.GetKeyDown(boostKey) && canBoost && rb != null)
        {
            // Default to the last direction faced.
            boostDirection = facingDirection;

            // Prefer the direction currently being held.
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                boostDirection = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                boostDirection = 1;
            }

            StartCoroutine(Boost());
        }
    }

    void FixedUpdate()
    {
        if (isBoosting && rb != null)
        {
            // Move horizontally and temporarily stop vertical movement.
            rb.linearVelocity = new Vector2(
                boostDirection * boostSpeed,
                0f
            );
        }
    }

    IEnumerator Boost()
    {
        if (rb == null)
        {
            yield break;
        }

        canBoost = false;
        isBoosting = true;

        // Temporarily disable gravity.
        rb.gravityScale = 0f;

        // Apply the initial boost velocity.
        rb.linearVelocity = new Vector2(
            boostDirection * boostSpeed,
            0f
        );

        // Maintain the boost for its duration.
        yield return new WaitForSeconds(boostDuration);

        // End the boost and restore gravity.
        isBoosting = false;
        rb.gravityScale = originalGravity;

        // Wait before allowing another boost.
        yield return new WaitForSeconds(cooldown);

        canBoost = true;
    }

    // Allows RunAnimation.cs to detect an active boost.
    public bool IsBoosting()
    {
        return isBoosting;
    }

    void OnDisable()
    {
        // Reset the boost state if this component is disabled.
        isBoosting = false;

        // Restore gravity so the player doesn't remain weightless.
        if (rb != null)
        {
            rb.gravityScale = originalGravity;
        }
    }
}
