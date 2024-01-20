using System.Collections;
using UnityEditor;
using UnityEngine;



public class Explosion : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 explodeDiretion = Vector3.zero;
    private float explodeSpeed = 200f;
    private float explodeRange = 2f;

    private Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, startPosition) >= explodeRange)
        {
            Destroy(gameObject);
        }
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        rb.velocity = explodeDiretion * explodeSpeed * Time.deltaTime;
    }

    public void SetExplosion(Vector3 diretion, float speed, float range)
    {
        explodeDiretion = diretion;
        explodeSpeed = speed;
        explodeRange = range;
    }


    /* TODO
     * A bomba nao se esta a detruir depois de sair na direçao da parede
     */
    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("Explosio has hit: " + other.gameObject + " with the tag of " + other.gameObject.tag);
        
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Bomb")
        {
            other.gameObject.GetComponent <Bomb>().Explode();
            Destroy(gameObject);
        }

    }
}