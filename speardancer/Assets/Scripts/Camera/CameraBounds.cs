
using UnityEngine;

public class CameraBoundsFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Camera Bounds")]
    public BoxCollider2D bounds;

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public Vector2 offset = new Vector2(0f, 1f);

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player == null || bounds == null || cam == null)
            return;

        // Calculate the camera's visible size.
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // Get the edges of the level bounds.
        Bounds level = bounds.bounds;

        // Find the player's desired camera position.
        Vector3 targetPosition = player.position;
        targetPosition.x += offset.x;
        targetPosition.y += offset.y;

        // Stop the camera from showing outside the level.
        float minX = level.min.x + halfWidth;
        float maxX = level.max.x - halfWidth;
        float minY = level.min.y + halfHeight;
        float maxY = level.max.y - halfHeight;

        // If the level is smaller than the camera view,
        // keep the camera centred on that axis.
        targetPosition.x = minX > maxX
            ? level.center.x
            : Mathf.Clamp(targetPosition.x, minX, maxX);

        targetPosition.y = minY > maxY
            ? level.center.y
            : Mathf.Clamp(targetPosition.y, minY, maxY);

        targetPosition.z = transform.position.z;

        // Smoothly follow the player within the boundaries.
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
