
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    [Header("Surface Detection")]
    public LayerMask stickSurfaceLayers;

    private Rigidbody2D rb;
    private bool isFlying;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void Launch(Vector2 direction, float speed)
    {
        if (rb == null)
        {
            Debug.LogError(
                "The spear needs a Rigidbody2D!"
            );
            return;
        }

        isFlying = true;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = direction.normalized * speed;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg + 90f;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFlying)
            return;

        // Do not stick to the player who threw it.
        if (other.GetComponentInParent<PlayerMovement>() != null)
            return;

        // Only stick to designated surfaces.
        bool isStickSurface =
            (stickSurfaceLayers.value
            & (1 << other.gameObject.layer)) != 0;

        if (!isStickSurface)
            return;

        isFlying = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Attach to the surface so the spear follows it if it moves.
        transform.SetParent(other.transform, true);
    }
}
