using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 1f;

    private bool isMoving = true;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            //transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * moveSpeed);
            rb.MovePosition(Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * moveSpeed));
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isMoving = false;
            //Debug.Log("Hit the player");
        }
        if (collision.gameObject.tag == "Bomb")
        {
            isMoving = false;
            //Debug.Log("Hit the bomb");
        }

    }
}
