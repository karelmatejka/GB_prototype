using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSaucer : MonoBehaviour {

    public GameObject SaucerPosition;
    public Animator SaucerAnimator;
    // Use this for initialization
    void Start () {
		
	}
	
    public void Landing(Vector3 position)
    {
        this.transform.position = position;
        this.transform.SetParent(MainScript.GetInstance().CameraAreaScript.transform);
        StartCoroutine(LandingAnimation());
    }

    public IEnumerator LandingAnimation()
    {
        int i;

        MainScript.GetInstance().Cutscene = true;

        Camera.main.transform.position = MainScript.GetInstance().CameraAreaScript.GetCameraPos();

        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Count; i++)
        {
            MainScript.GetInstance().PlayersToFollow[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(2.5f);
        SaucerPosition.transform.localPosition = Vector3.forward * 2;

        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Count; i++)
        {
            MainScript.GetInstance().PlayersToFollow[i].gameObject.SetActive(true);
            MainScript.GetInstance().PlayersToFollow[i].transform.position = MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].StartingPosition.transform.position + Vector3.up * 5;
            MainScript.GetInstance().PlayersToFollow[i].SetJump(false);
        }

        yield return new WaitForSeconds(0.3f);

        SaucerPosition.transform.localPosition = Vector3.forward * 20;

        MainScript.GetInstance().Cutscene = false;

        yield return new WaitForSeconds(0.3f);

        SaucerAnimator.SetTrigger("CloseDoor");
        yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
