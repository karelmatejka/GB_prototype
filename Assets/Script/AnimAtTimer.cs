using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAtTimer : MonoBehaviour {

    Animation anim;
    SequenceTimer SequenceTimerScript;

    // Use this for initialization
    void Start () {
        anim = this.GetComponent<Animation>();
        SequenceTimerScript = this.GetComponent<SequenceTimer>();
    }
	
	// Update is called once per frame
	void Update () {

        if (SequenceTimerScript != null && SequenceTimerScript.isTriggered())
        {
            anim.Stop(anim.clip.name);
            anim.Play(anim.clip.name);
            //Debug.Log("Playing Animation At Timer");
        }
        
    }
}
