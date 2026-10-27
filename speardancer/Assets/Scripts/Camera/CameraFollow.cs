
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Player Target")]
    public Transform player;

    [Header("Camera Following")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed = 5f;

    [Header("Zoom Settings")]
    public bool enableZoom = true;
    public KeyCode zoomKey = KeyCode.C;
    public float normalZoom = 5f;
    public float zoomedInSize = 3f;
    public float zoomSmoothSpeed = 5f;

    [Header("Vertical Camera View")]
    public bool enableVerticalView = true;
    public KeyCode verticalViewKey = KeyCode.V;
    public float verticalViewOffset = 4f;
    public float verticalViewSmoothSpeed = 5f;

    [Header("Optional Camera Bounds")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Camera cam;
    private bool isZoomedIn = false;
    private bool isLookingUp = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam != null)
        {
            normalZoom = cam.orthographicSize;
        }
    }

    private void Update()
    {
        // Toggle zoom
        if (enableZoom && Input.GetKeyDown(zoomKey))
        {
            isZoomedIn = !isZoomedIn;
        }

        // Toggle vertical camera view
        if (enableVerticalView &&
            Input.GetKeyDown(verticalViewKey))
        {
            isLookingUp = !isLookingUp;
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        // Calculate the camera's target position
        Vector3 targetPosition = player.position + offset;

        // Shift the camera view vertically
        if (enableVerticalView && isLookingUp)
        {
            targetPosition.y += verticalViewOffset;
        }

        // Optional camera bounds
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                minBounds.x,
                maxBounds.x
            );

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                minBounds.y,
                maxBounds.y
            );
        }

        // Smoothly follow the player
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        // Smoothly zoom in and out
        if (cam != null && enableZoom)
        {
            float targetSize = isZoomedIn
                ? zoomedInSize
                : normalZoom;

            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetSize,
                zoomSmoothSpeed * Time.deltaTime
            );
        }
    }
}
