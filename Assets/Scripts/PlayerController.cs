using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float xMoveSpeed;
    [SerializeField] float yMoveSpeed;
    [SerializeField] float zMoveSpeed;
    [SerializeField] private float destroyTimer = 2f;
    private int maxBomb = 1;
    private int bombPlaced = 0;
    private bool hasControl = true;
    private bool isPaused = false;


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
        if(hasControl && !isPaused)
        {
            Movement();
            PlaceBomb();
        }
    }

    private void Movement()
    {
        Vector3 newVelocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            newVelocity += new Vector3(0f, 0f, zMoveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            newVelocity += new Vector3(0f, 0f, -zMoveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            newVelocity += new Vector3(-xMoveSpeed, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            newVelocity += new Vector3(xMoveSpeed, 0f, 0f);
        }

        rb.velocity = newVelocity;
    }

    private void PlaceBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombPlaced < maxBomb)
        {
            //agora vai colocar as bombas dentro dos NODES corretamente
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.position = new Vector3(Mathf.Round(transform.position.x), 0.5f, Mathf.Round(transform.position.z));
            bombPlaced++;
        }
    }

    public void Died()
    {
        //Tell the GAme manager that player died
        myGameManager.PlayerDied();
        hasControl= false;
        //Destrui o objeto
        Destroy(gameObject, destroyTimer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Died();          
        }
    }

    public void BombExploded()
    {
        bombPlaced--; 
    }

    public float GetDestroyerTime()
    {
        return destroyTimer;
    }

    public void initializePlayer(int bombs)
    {
        maxBomb = bombs;
    }

    public void SetPaused(bool state)
    {
        isPaused = state;
    }
}
