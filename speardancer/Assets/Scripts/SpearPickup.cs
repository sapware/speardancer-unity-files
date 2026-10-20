
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
        canBeCollected = false;

        yield return new WaitForSeconds(pickupDelay);

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
