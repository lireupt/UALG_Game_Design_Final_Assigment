using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    
    [SerializeField] float xMoveSpeed;
    [SerializeField] float yMoveSpeed;
    [SerializeField] float zMoveSpeed;

    Rigidbody rb;

    [SerializeField] GameObject bombPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        PlaceBomb();
    }

    private void Movement()
    {
        Vector3 newVelocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            newVelocity = new Vector3(0f, 0f, zMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            newVelocity = new Vector3(0f, 0f, -zMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            newVelocity = new Vector3(-xMoveSpeed, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            newVelocity = new Vector3(xMoveSpeed, 0f, 0f);
        }

        rb.velocity = newVelocity;

        //transform.position = transform.position + (newPosition * Time.deltaTime);
    }

    private void PlaceBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Bomba");

            
            //TODO tenho de ver esta parte, porque ele instancia e passa o player sempre para a direita
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            //bomb.transform.position = transform.position;
        }


    }
    
}
