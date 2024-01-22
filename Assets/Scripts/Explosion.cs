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

    private void OnTriggerEnter(Collider other)
    {
              
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Bomb" )
        {
            // Verifica se o objeto possui o componente necessário antes de acessá-lo
            Bomb bomb = other.gameObject.GetComponent<Bomb>();
            if (bomb != null)
            {
                // Faça alguma coisa com a bomba
                Destroy(gameObject);
            }
        }

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Died();
          //  Destroy(gameObject);
        }
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().Died();
            //  Destroy(gameObject);
        }
        if (other.gameObject.tag == "DestrWall")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}