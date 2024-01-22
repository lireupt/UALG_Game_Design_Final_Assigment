using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    PlayerController player;
    private float explosionTimer = 0;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explodeDelay = 2f;  
    [SerializeField] private float explodeSpeed = 200f;
    private int explodeRange = 1;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //Debug.Log("O nome do objecto +e : " + player.name);
        explodeRange = FindAnyObjectByType<GameManager>().GetExplodeRange();
       
    }

    // Update is called once per frame
    void Update()
    {
        explosionTimer += Time.deltaTime;
        if (explosionTimer >= explodeDelay)
        {
            Explode();         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GetComponent<SphereCollider>().isTrigger = false;
        }
    }

    public void Explode()
    {
        GameObject explosionRight = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionRight.GetComponent<Explosion>().SetExplosion(Vector3.right, explodeSpeed, explodeRange);

        GameObject explosionLeft = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionLeft.GetComponent<Explosion>().SetExplosion(Vector3.left, explodeSpeed, explodeRange);


        GameObject explosionUp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionUp.GetComponent<Explosion>().SetExplosion(Vector3.forward, explodeSpeed, explodeRange);

        GameObject explosionDown = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionDown.GetComponent<Explosion>().SetExplosion(Vector3.back, explodeSpeed, explodeRange);

        player.BombExploded();
        Destroy(gameObject);       
    }
}
