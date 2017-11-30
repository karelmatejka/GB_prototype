using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTimer : MonoBehaviour {
    public float LoopTime;
    public float TimeShift;

    [System.Serializable]
    public struct TimeTrigger
    {
        public float Time;
        [HideInInspector] public bool Triggered;
    }
    public TimeTrigger[] TimeTriggers;

    [HideInInspector] public bool Triggered = false;
    float ActualTime;
    bool FirstTime = true;
	// Use this for initialization
	void Start ()
    {
        Init();
	}

    public void  Init()
    {
        ActualTime = TimeShift % LoopTime;
        if (ActualTime < 0)
        {
            ActualTime = LoopTime + ActualTime;
        }
        //Debug.Log("Sequence Timer Init Time: " + ActualTime);

        if (TimeShift>0) FirstTime = true;
        else FirstTime = false;

        ResetTriggers();
    }

    void ResetTriggers()
    {
        int i;

        for (i = 0; i < TimeTriggers.Length; i++)
        {
            TimeTriggers[i].Triggered = false;
        }
        
    }

    public bool isTriggered()
    {
        bool ret;
        ret = Triggered;
        Triggered = false;
        return ret;
    }

    // Update is called once per frame
    void Update () {
        int i;

        ActualTime += Time.deltaTime * 1000;
        if (ActualTime > LoopTime)
        {
            ActualTime -= LoopTime;
            ResetTriggers();
            FirstTime = false;
        }

        for (i = 0; i < TimeTriggers.Length; i++)
        {
            if (ActualTime >= TimeTriggers[i].Time && !TimeTriggers[i].Triggered && !FirstTime)
            {
                TimeTriggers[i].Triggered = true;
                Triggered = true;
                //Debug.Log("Triggered at Time: " + ActualTime);
            }
        }
    }
}
