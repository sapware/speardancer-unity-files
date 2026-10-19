using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility  : MonoBehaviour
{
    public GameObject spearPrefab; // Drag your prefab into this field in the inspector
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            spearPrefab.SetActive(!spearPrefab.activeSelf); // Toggle active state
        }
    }
}
