using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroy : MonoBehaviour {

	// Auto destroys the particle system after it is done.
	void Start ()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration + 2.0f);
	}
}
