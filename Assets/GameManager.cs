using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private int lives = 3;
    [SerializeField] private GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(playerPrefab, new Vector3(1,0.5f,1), Quaternion.identity );
     

    }

    public void PlayerDied()
    {
        if ( lives > 1 )
        {
            lives--;
        }
        else
        {
            Debug.Log("Game Over");
        }
        Debug.Log("Player died");
    }
}
