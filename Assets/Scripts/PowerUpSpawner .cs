using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] powerUpPrefabs;

    public void BlockDestroyed(Vector3 pos)
    {
        // Add a Y-axis offset to raise it
        float yOffset = 0.5f; // Adjust this value as needed

        // Modify the position on the Y-axis
        pos.y += yOffset;

        // Instantiate the object with the new position
        Instantiate(powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)], pos, Quaternion.identity);
    }
}
