using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUInterface : MonoBehaviour {

    public Text DisplayCoins;
    public Image FadeImage;
    public Image FadeImage2;

    public GameObject GameGui;
    public GameObject[] ActiveFuel;
    public GameObject[] InactiveFuel;
    public RectTransform FloatingFuelInstance;
    public Animator FloatingFuelAnimator;
    float FloatingFuelTime;
    int FloatingFuelIndex;
    bool IsFuelFloating;
    float FloatingFuelEndTime = 1f;
    Vector2 FloatingFuelStartPos;
    Vector2 FloatingFuelEndPos;


    [HideInInspector] public bool fading = false;
    float finalfade = 0;
    float actualfade = 0;
    int fadedirection = 1;

    RectTransform rectTransform;
    //Vector2 uiOffset;

    public Vector2 Get3dTo2d(Vector3 objectTransformPosition)
    {
        // Get the position on the canvas
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectTransformPosition);
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * rectTransform.sizeDelta.x, - (1 - ViewportPosition.y) * rectTransform.sizeDelta.y);

        //Debug.Log("ViewportPosition: " + ViewportPosition);
        //Debug.Log("proportionalPosition: " + proportionalPosition);

        // Set the position and remove the screen offset
        return proportionalPosition;
    }



    // Use this for initialization
    void Start ()
    {
        FadeImage.material.SetFloat("_Cutoff", 1);
        FadeImage2.material.SetFloat("_Cutoff", 1);

        FloatingFuelAnimator.gameObject.SetActive(false);

        // Get the rect transform
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void InitGui()
    {
        //display gui
        if (MainScript.GetInstance().LoaderInstance != null && MainScript.GetInstance().LoaderInstance.activemenu == -1)
        {
            GameGui.gameObject.SetActive(true);
        }
        else
        {
            GameGui.gameObject.SetActive(false);
        }

        AddCoins(0);
    }

    public void AddCoins(int amount)
    {
      
        if (MainScript.GetInstance().LoaderInstance != null && MainScript.GetInstance().LoaderInstance.activemenu == -1)
        {
            MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].CollectedCoins += amount;
            DisplayCoins.text = MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].CollectedCoins + "/" + MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].CoinsDef.Count;
        }
      
    }

    // Update is called once per frame
    void Update ()
    {
	    if (fading)
        {
            actualfade += Time.unscaledDeltaTime * fadedirection * 4;
            if ((fadedirection > 0 && actualfade > finalfade) || (fadedirection < 0 && actualfade < finalfade))
            {
                //Debug.Log("Fading End");
                actualfade = finalfade;
                fading = false;
            }
            //Color newColor = new Color(0f, 0f, 0f, actualfade);
            //FadeImage.color = newColor;
            FadeImage.material.SetFloat("_Cutoff", 1 - actualfade + 0.2f);
            FadeImage2.material.SetFloat("_Cutoff", 1 - actualfade);
        }

        if (IsFuelFloating)
        {
            FloatingFuelTime += Time.deltaTime;

            if (FloatingFuelTime > FloatingFuelEndTime)
            {
                IsFuelFloating = false;
                ActiveFuel[FloatingFuelIndex - 1].gameObject.SetActive(true);
                InactiveFuel[FloatingFuelIndex - 1].gameObject.SetActive(false);
                FloatingFuelAnimator.SetTrigger("End");
                FloatingFuelTime = FloatingFuelEndTime;
            }

            FloatingFuelInstance.position = FloatingFuelStartPos + (FloatingFuelEndPos - FloatingFuelStartPos) * FloatingFuelTime * FloatingFuelTime * FloatingFuelTime / FloatingFuelEndTime;
        }

    }

    void SetRandomFadePosition()
    {
        FadeImage2.transform.position = new Vector3(Random.Range(0, 600) - 300, Random.Range(0, 600) - 300, 0);
        FadeImage.transform.position = FadeImage2.transform.position + Vector3.back;
    }

    public void Fade(bool fadein)
    {
        if (fadein)
        {
            SetRandomFadePosition();
            fadedirection = 1;
            finalfade = 1.5f;
            fading = true;
        }
        else
        {
            SetRandomFadePosition();
            fadedirection = -1;
            finalfade = -0.5f;
            fading = true;
        }
    }

    public void CollectFuelIndicator(int collected, Vector3 startingPosition)
    {

        FloatingFuelIndex = collected;
        FloatingFuelStartPos = Get3dTo2d(startingPosition);
        FloatingFuelEndPos = InactiveFuel[FloatingFuelIndex - 1].transform.position;
        FloatingFuelInstance.anchoredPosition = FloatingFuelStartPos;
        FloatingFuelStartPos = FloatingFuelInstance.transform.position;
        FloatingFuelAnimator.gameObject.SetActive(true);
        FloatingFuelAnimator.SetTrigger("Start");
        FloatingFuelTime = 0;
        IsFuelFloating = true;
    }

    public void ResetFuelIndicators(int number)
    {
        int i;
        for (i = 0; i < ActiveFuel.Length; i++)
        {
            ActiveFuel[i].gameObject.SetActive(false);
            if (i < number)
            {
                InactiveFuel[i].gameObject.SetActive(true);
            }
            else
            {
                InactiveFuel[i].gameObject.SetActive(false);
            }
        }
    }
}
