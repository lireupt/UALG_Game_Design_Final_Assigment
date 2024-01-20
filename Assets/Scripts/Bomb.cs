using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    PlayerContoller player;
    private float explosionTimer = 0;

    [SerializeField] private float explodeDelay = 2f;  
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explodeSpeed = 200f;
    [SerializeField] private float explodeRange = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContoller>();
        //Debug.Log("O nome do objecto +e : " + player.name);
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
        //Debug.Log("Aqui dentro");
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

        Destroy(gameObject);
    }
}
