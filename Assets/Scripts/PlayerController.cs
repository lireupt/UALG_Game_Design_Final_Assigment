using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float moveSpeed = 0;

    private int maxBomb = 1;
    private int bombPlaced = 0;
    private bool hasControl = true;
    private bool isPaused = false;
    private AudioSource myAudioSource;

    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private float destroyTimer = 2f;

    Rigidbody rb;

    [SerializeField] GameObject bombPrefab;

    private GameManager myGameManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myGameManager = FindObjectOfType<GameManager>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasControl && !isPaused)
        {
            Movement();
            Rotation();
            PlaceBomb();
        }
    }

    public void Rotation()
    {
        if (rb.velocity != Vector3.zero)
        {
            transform.forward = rb.velocity;
        }
    }

    private void Movement()
    {
        Vector3 newVelocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            newVelocity += new Vector3(0f, 0f, moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            newVelocity += new Vector3(0f, 0f, -moveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            newVelocity += new Vector3(-moveSpeed, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            newVelocity += new Vector3(moveSpeed, 0f, 0f);
        }

        rb.velocity = newVelocity;
    }

    private void PlaceBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombPlaced < maxBomb)
        {
            // Check if there is already a bomb at the location, preventing the instantiation of another bomb
            Vector3 center = new Vector3(Mathf.Round(transform.position.x), 0.5f, Mathf.Round(transform.position.z));
            Collider[] hitColliders = Physics.OverlapSphere(center, 0.5f);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.tag == "Bomb")
                {
                    return;
                }
            }

            // Place bombs within the correct NODES
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.position = center;
            bombPlaced++;
        }
    }

    public void Died()
    {
        // Tell the Game manager that the player died
        myGameManager.PlayerDied();
        hasControl = false;
        // Destroy the object
        Destroy(gameObject, destroyTimer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
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

    public void InitializePlayer(int bombs, float speed)
    {
        maxBomb = bombs;
        moveSpeed = speed;
    }

    public void SetPaused(bool state)
    {
        isPaused = state;
    }

    // TODO: Not working, needs to be revised
    public void PlayPowerUpSound()
    {
        // Debug.Log("PlayPowerUpSound called");
        if (myAudioSource != null && powerUpSound != null)
        {
            myAudioSource.PlayOneShot(powerUpSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or PowerUpSound is null. Check your assignments.");
        }
    }
}
