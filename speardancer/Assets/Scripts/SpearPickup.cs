
using UnityEngine;

public class SpearPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            Destroy(gameObject);
        }
    }
}
