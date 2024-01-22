using System.Collections;
using UnityEngine;


public class TesteCollider : MonoBehaviour
{
    private Rigidbody rb;

    // Use this for initialization
    void Start()
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
            //isMoving = false;
            Debug.Log("Hit the player como collider");
        }
        if (other.gameObject.tag == "Bomb")
        {
            //isMoving = false;
            //Debug.Log("Hit the bomb");
        }
        if (other.gameObject.tag == "DestrWall")
        {
            //isMoving = false;
            Debug.Log("Explosio has hit: " + other.gameObject + " with the tag of " + other.gameObject.tag);
        }

    }

}
