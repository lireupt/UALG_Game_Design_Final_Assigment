using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private bool isMoving = true;
    //[SerializeField] private float mindelayTime = 0.25f;
    //[SerializeField] private float maxdelayTime = 0.3f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //isMoving = false;
            Debug.Log("Hit the player como trigguer");
        }
    }
   
    public void OnColliderEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hit the player como collider");
            isMoving = false;
            
        }
        if (other.gameObject.tag == "Bomb")
        {
            Debug.Log("Hit the bomb");
            //isMoving = false;
            
        }
        if (other.gameObject.tag == "DestrWall")
        {
            
            Debug.Log("Explosio has hit: " + other.gameObject + " with the tag of " + other.gameObject.tag);
        }
    }

    public void Died()
    {
        GameManager myGamerManager = FindObjectOfType<GameManager>();
        myGamerManager.EnemyHasDied();



        /*
         //Controlador de ganhar do inimigo se o meu plyer matar o inimigo, desativado pk ainda falta optimizar a busca no ASTAR
        ele ainda faz a busca e esta sempre a perder
        
        myGamerManager.WinPanel();
         
         */
        Destroy(gameObject);
    }
}
