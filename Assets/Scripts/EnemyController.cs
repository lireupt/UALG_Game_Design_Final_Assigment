using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private bool isMoving = true;
    //[SerializeField] private float mindelayTime = 0.25f;
    //[SerializeField] private float maxdelayTime = 0.3f;

    Rigidbody rb;
    AudioSource myAudioSource;

    [SerializeField] private float destroyTimer = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        Rotation();
    }

    // Function to handle the rotation of the enemy towards its movement direction
    public void Rotation()
    {
        transform.LookAt(rb.position);
    }

    // Triggered when the enemy collides with another collider (useful for detecting the player)
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // isMoving = false;
            Debug.Log("Hit the player as trigger");
        }
    }

    // Triggered when the enemy collides with another collider (useful for detecting the player and other objects)
    // Need to adjust this part to deflect enemy bombs and the like
    /*
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Debug.Log("Hit the player as collider");
            // isMoving = false;
        }
        if (other.gameObject.tag == "Bomb")
        {
            // Debug.Log("===========>>HIT THE BOMB");
            // isMoving = false;
        }
        if (other.gameObject.tag == "DestrWall")
        {
            // Debug.Log("Explosion has hit: " + other.gameObject + " with the tag of " + other.gameObject.tag);
        }
    }*/


    // Called when the enemy dies
    public void Died()
    {
        GameManager myGameManager = FindObjectOfType<GameManager>();
        myGameManager.EnemyHasDied();

        // Play audio when the enemy dies
        myAudioSource.Play();

        /*
         // Controller for winning if the player kills the enemy, disabled because ASTAR search optimization is still pending
         he still does the search and is always losing
         
        myGameManager.WinPanel();
         
         */
        // Destroy the enemy object after a certain delay
        Destroy(gameObject, destroyTimer);
    }
}
