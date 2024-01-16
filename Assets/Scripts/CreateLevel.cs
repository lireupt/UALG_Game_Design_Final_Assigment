using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLevel : MonoBehaviour
{
    public Vector3 start;
    public Vector3 offset;
    [Header("Prefabs")]
    public GameObject wall;
    public GameObject innerWall;
    public GameObject destructableWall;
    public GameObject ground;
    [Header("PlaceHolder")]
    public Transform groundHolder;
    public Transform outerWallHolder;
    public Transform innerWallHolder;
    public Transform destructableHolder;
    [Header("Set Grid Size > 5")]
    public int gridSizeX;
    public int gridSizeZ;
    [Header("LayerMask")]
    public LayerMask layerMask;
    [Header("Camera")]
    public Camera mainCamera;



    public static CreateLevel Instance; // Singleton instance



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }


}
