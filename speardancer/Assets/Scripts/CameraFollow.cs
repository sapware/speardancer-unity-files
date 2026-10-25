
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Map Boundaries")]
    public BoxCollider2D leftBoundary;
    public BoxCollider2D rightBoundary;
    public BoxCollider2D topBoundary;
    public BoxCollider2D bottomBoundary;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null || cam == null)
            return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float x = player.position.x;
        float y = player.position.y;

        // Stop the visible left side at the left map edge.
        if (leftBoundary != null)
            x = Mathf.Max(x, leftBoundary.bounds.max.x + halfWidth);

        // Stop the visible right side at the right map edge.
        if (rightBoundary != null)
            x = Mathf.Min(x, rightBoundary.bounds.min.x - halfWidth);

        // Stop the visible bottom side at the bottom map edge.
        if (bottomBoundary != null)
            y = Mathf.Max(y, bottomBoundary.bounds.max.y + halfHeight);

        // Stop the visible top side at the top map edge.
        if (topBoundary != null)
            y = Mathf.Min(y, topBoundary.bounds.min.y - halfHeight);

        transform.position = new Vector3(
            x,
            y,
            transform.position.z
        );
    }
}
