using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    enum PowerUps {MaxBombs, Range, Speed}
    [SerializeField] PowerUps powerUpType;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag == "Player")
        {
            if(powerUpType == PowerUps.MaxBombs)
            {
                //incrise maxbombs
                FindAnyObjectByType<GameManager>().IncreaseMaxBombs();
                Destroy(gameObject);
             
            }else if(powerUpType == PowerUps.Range)
            {
                //incrise range
                FindAnyObjectByType<GameManager>().IncreaseExplodeRange();
                Destroy(gameObject);
                
            }
            else if(powerUpType == PowerUps.Speed){
                //incrise speed
                FindAnyObjectByType<GameManager>().IncreaseSpeed();
                Destroy(gameObject);
            }
        }
    }
}
