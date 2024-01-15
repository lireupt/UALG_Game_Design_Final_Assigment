using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    PlayerContoller player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContoller>();
        Debug.Log("O nome do objecto +e : " + player.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Aqui dentro");
        GetComponent<SphereCollider>().isTrigger = false;

    }
}
