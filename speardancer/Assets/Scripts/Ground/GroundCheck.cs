
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Detection")]
    public Transform checkPoint;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Status")]
    public bool isGrounded;

    void Update()
    {
        CheckGround();
    }

    public void CheckGround()
    {
        if (checkPoint == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            checkPoint.position,
            checkRadius,
            groundLayer
        ) != null;
    }

    void OnDrawGizmosSelected()
    {
        if (checkPoint == null)
            return;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            checkPoint.position,
            checkRadius
        );
    }
}
