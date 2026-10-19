
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    [Header("Surface Detection")]
    public LayerMask stickSurfaceLayers;

    private Rigidbody2D rb;
    private bool isFlying;
    private bool hasStuck;

    public bool IsStuck => hasStuck;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("The spear needs a Rigidbody2D!");
            return;
        }

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Launch(Vector2 direction, float speed)
    {
        if (rb == null || hasStuck)
            return;

        isFlying = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = direction.normalized * speed;

        // Keep the current 180-degree rotation offset.
        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg + 180f;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFlying || hasStuck)
            return;

        // Ignore the player who threw the spear.
        if (other.GetComponentInParent<PlayerMovement>() != null)
            return;

        // Only stick to selected surface layers.
        int otherLayerMask = 1 << other.gameObject.layer;

        if ((stickSurfaceLayers.value & otherLayerMask) == 0)
            return;

        hasStuck = true;
        isFlying = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.SetParent(other.transform, true);

        Debug.Log("Spear stuck to: " + other.gameObject.name);
    }
}
