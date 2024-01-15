using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public GameObject bomb;
    public float power = 10.0f;
    public float radius = 5.0f;
    public float upForce = 1.0f;

    public GameObject explodePrefab;
    PlayerContoller player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContoller>();
        //Debug.Log("O nome do objecto +e : " + player.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bomb == enabled)
        {
            Invoke("Detonate", 5);
        }
        
    }

    //Put bombs in place without move
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Aqui dentro");
        GetComponent<SphereCollider>().isTrigger = false;

    }



    void Detonate()
    {
        Instantiate(explodePrefab, transform.position, transform.rotation);

        Vector3 explosionPosition = bomb.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(power, explosionPosition, radius, upForce, ForceMode.Impulse);
            }  
        }
        Destroy(gameObject);
    }
}
