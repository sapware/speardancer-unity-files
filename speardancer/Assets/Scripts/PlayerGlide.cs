
using UnityEngine;

public class PlayerGlide : MonoBehaviour
{
    [Header("Glide Settings")]
    public float glideFallSpeed = 2f;

    [Header("Controls")]
    public KeyCode glideKey = KeyCode.G;

    private Rigidbody2D rb;
    private bool isGliding;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Glide only while airborne and holding G.
        isGliding = !IsGrounded() && Input.GetKey(glideKey);
    }

    void FixedUpdate()
    {
        if (rb == null || !isGliding)
            return;

        // Limit downward speed without affecting horizontal movement.
        if (rb.linearVelocity.y < -glideFallSpeed)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -glideFallSpeed
            );
        }
    }

    bool IsGrounded()
    {
        // Cast downward to detect the ground.
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            0.7f
        );

        return hit.collider != null &&
               hit.collider.gameObject != gameObject &&
               hit.collider.transform.root != transform;
    }
}
