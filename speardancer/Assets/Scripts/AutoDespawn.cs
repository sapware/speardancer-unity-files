
using UnityEngine;
public class AutoDespawn : MonoBehaviour
{
    public float lifetime = 15f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}