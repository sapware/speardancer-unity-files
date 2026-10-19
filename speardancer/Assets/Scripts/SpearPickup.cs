
using UnityEngine;
using System.Collections;

public class SpearPickup : MonoBehaviour
{
    public float pickupDelay = 5f;

    private bool canBeCollected = false;

    void Start()
    {
        StartCoroutine(EnablePickupAfterDelay());
    }

    IEnumerator EnablePickupAfterDelay()
    {
        // Prevent the player from collecting the spear immediately.
        canBeCollected = false;

        yield return new WaitForSeconds(pickupDelay);

        // The spear can now be collected.
        canBeCollected = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBeCollected)
            return;

        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            Destroy(gameObject);
        }
    }
}
