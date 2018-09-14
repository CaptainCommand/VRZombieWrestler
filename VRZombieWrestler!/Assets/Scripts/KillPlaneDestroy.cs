using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlaneDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        // Kills Zombies that fall into the kill plane.
        if (collision.gameObject.CompareTag("Zombie"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(1000.0f);
        }
    }
}

