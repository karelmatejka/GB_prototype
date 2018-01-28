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
        public int TriggerId;
        [HideInInspector] public bool Triggered;
    }
    public TimeTrigger[] TimeTriggers;

    float ActualTime;
    bool FirstTime = true;

    [System.Serializable]
    public struct Shooting
    {
        public GameObject ProjectilePrefab;
        public int TriggerID;
    }

    public Shooting[] ShootingEvent;

    [System.Serializable]
    public struct Sound
    {
        public AudioSource[] ShotSounds;
        public int TriggerID;
    }

    public Sound[] SoundEvent;

    [System.Serializable]
    public struct Anim
    {
        public Animation anim;
        public AnimationClip clip;
        public int TriggerID;
    }
    public Anim[] AnimEvent;

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

    

    // Update is called once per frame
    void Update () {
        int i;
        int j;

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
                

                for (j = 0; j < ShootingEvent.Length; j++)
                {
                    if (ShootingEvent[j].TriggerID == TimeTriggers[i].TriggerId)
                    {
                        MainScript.GetInstance().InstantiateObject(ShootingEvent[j].ProjectilePrefab, this.transform.position, this.transform.rotation);
                    }
                }

                for (j = 0; j < SoundEvent.Length; j++)
                {
                    if (SoundEvent[j].TriggerID == TimeTriggers[i].TriggerId)
                    {
                        MainScript.GetInstance().PlayRandomSound(SoundEvent[j].ShotSounds, this.transform.position, true);
                    }
                }

                for (j = 0; j < AnimEvent.Length; j++)
                {
                    if (AnimEvent[j].TriggerID == TimeTriggers[i].TriggerId)
                    {
                        AnimEvent[j].anim.Stop(AnimEvent[j].anim.clip.name);
                        AnimEvent[j].anim.Play(AnimEvent[j].clip.name);
                    }
                }


            }
        }

        
    }
}
