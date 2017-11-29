using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAtTimer : MonoBehaviour {

    SequenceTimer SequenceTimerScript;
    public GameObject ProjectilePrefab;
    // Use this for initialization
    void Start () {
        SequenceTimerScript = this.GetComponent<SequenceTimer>();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (SequenceTimerScript != null && SequenceTimerScript.isTriggered())

        {
            GameObject projectile;
            projectile = MainScript.GetInstance().InstantiateObject(ProjectilePrefab, this.transform.position,this.transform.rotation);
            Debug.Log("Shooting At Timer");
        }
    }
}
