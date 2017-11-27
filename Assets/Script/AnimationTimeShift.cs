using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTimeShift : MonoBehaviour {

    public float TimeShift;
    public bool RandomShift = false;
    Animation anim;

    // Use this for initialization
    void Start () {
		anim = this.GetComponent<Animation>();
        if (!RandomShift)
        {
            anim[anim.clip.name].time = TimeShift / 1000;
        }
        else
        {
            anim[anim.clip.name].time = Random.value * TimeShift / 1000;
        }
    }
	
}
