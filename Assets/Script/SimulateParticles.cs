using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateParticles : MonoBehaviour {

    ParticleSystem ps;

	// Use this for initialization
	void Start ()
    {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        ps.Simulate(Time.unscaledDeltaTime, true, false);
    }
}
