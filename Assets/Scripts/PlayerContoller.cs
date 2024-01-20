using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{

    public AStar aStar;

    [SerializeField] float xMoveSpeed;
    [SerializeField] float yMoveSpeed;
    [SerializeField] float zMoveSpeed;

    Rigidbody rb;

    [SerializeField] GameObject bombPrefab;

    private GameManager myGameManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myGameManager = FindObjectOfType<GameManager>();
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
            //agora vai colocar as bombas dentro dos NODES corretamente
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.position = new Vector3(Mathf.Round(transform.position.x), 0.5f, Mathf.Round(transform.position.z));
        }
    }

    private void PlayerDied()
    {
       
    }

    private void Died()
    {
        //Tell the GAme manager that player died
        myGameManager.PlayerDied();
        //PLayer deth animation
        //remover player of the scene
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Died();
        }
    }

}
