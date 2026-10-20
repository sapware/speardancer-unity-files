using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    public Transform warpDestination;
    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = warpDestination.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}