using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUInterface : MonoBehaviour {

    public Text DisplayCoins;
    public Image FadeImage;


    public bool fading = false;
    float finalfade = 0;
    float actualfade = 0;
    int fadedirection = 1;

    // Use this for initialization
    void Start ()
    {
        
	}

    public void InitGui()
    {
        //display gui
        AddCoins(0);
    }

    public void AddCoins(int amount)
    {
        MainScript.GetInstance().Coins += amount;
        DisplayCoins.text = "" + MainScript.GetInstance().Coins;
    }

    // Update is called once per frame
    void Update ()
    {
	    if (fading)
        {
            actualfade += Time.unscaledDeltaTime * fadedirection * 3;
            if ((fadedirection > 0 && actualfade > finalfade) || (fadedirection < 0 && actualfade < finalfade))
            {
                Debug.Log("Fading End");
                actualfade = finalfade;
                fading = false;
            }
            Color newColor = new Color(0f, 0f, 0f, actualfade);
            FadeImage.color = newColor;
        }
	}
    public void Fade(bool fadein)
    {
        if (fadein)
        {
            fadedirection = 1;
            finalfade = 1;
            fading = true;
        }
        else
        {
            fadedirection = -1;
            finalfade = 0;
            fading = true;
        }
    }
}
