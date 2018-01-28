using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSaucer : MonoBehaviour {

    public GameObject SaucerPosition;
    public Animator SaucerAnimator;

    [System.Serializable]
    public struct FuelIndicator
    {
        public GameObject SlotSetup;
        public Animator[] slot;
    }

    public FuelIndicator[] FuelIndicators;

    // Use this for initialization
    void Start () {
		
	}

    public void CollectFuelIndicator(int collected, int all)
    {
        FuelIndicators[all - 1].slot[collected - 1].SetTrigger("On");
    }

    public void ResetFuelIndicators(int all)
    {
        int i;
        for (i = 0; i < FuelIndicators[all - 1].slot.Length; i++)
        {
            FuelIndicators[all - 1].slot[i].SetTrigger("Off");
        }
        MainScript.GetInstance().GuiInstance.ResetFuelIndicators(all);
    }

    public void ShowFuelIndicators(int number)
    {
        int i;
        for (i = 0; i < FuelIndicators.Length; i++)
        {
            FuelIndicators[i].SlotSetup.gameObject.SetActive(false);
        }

        FuelIndicators[number - 1].SlotSetup.gameObject.SetActive(true);
        MainScript.GetInstance().GuiInstance.ResetFuelIndicators(number);
    }

    public void OpenDoor()
    {
        SaucerAnimator.SetTrigger("OpenDoor");
    }

    public void CloseDoor()
    {
        SaucerAnimator.SetTrigger("CloseDoor");
    }

    public void Landing(Vector3 position)
    {
        this.transform.position = position;
        this.transform.SetParent(MainScript.GetInstance().CameraAreaScript.transform);
        StartCoroutine(LandingAnimation());
    }

    public IEnumerator FlyAwayAnimation()
    {
        MainScript.GetInstance().Cutscene = true;
        MainScript.GetInstance().PlayersToFollow[0].MoveToPosition(this.transform.position,1);
        Debug.Log("MOVING TO SHIP");

        yield return new WaitUntil(() => MainScript.GetInstance().PlayersToFollow[0].IsMovingToPosition == false);

        SaucerPosition.transform.localPosition = Vector3.forward * 2;

        Debug.Log("JUMP TO SHIP");
        MainScript.GetInstance().PlayersToFollow[0].ButtonFire = 2;
        yield return new WaitForSeconds(0.5f);
        MainScript.GetInstance().PlayersToFollow[0].PlayerBody.gameObject.SetActive(false);
        CloseDoor();
        //MainScript.GetInstance().InitLevel(false);
        MainScript.GetInstance().LoaderInstance.OpenMenu(4);
        Time.timeScale = 0.0f;
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

        //MainScript.GetInstance().PlayersToFollow[0].MoveToPosition(MainScript.GetInstance().GetPlayerStartPos(MainScript.GetInstance().PlayersToFollow[0], true),0.5f);

        MainScript.GetInstance().Cutscene = false;

        yield return new WaitForSeconds(0.3f);

        CloseDoor();
    }
}
