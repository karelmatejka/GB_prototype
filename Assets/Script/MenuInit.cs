using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuInit : MonoBehaviour {

    public UnityEngine.UI.Button firstbutton;
    [HideInInspector] public UnityEngine.UI.Button previousbutton = null;
    public UnityEngine.UI.Button[] buttons;


    void Start()
    {
        Canvas uiCanvas;
        uiCanvas = this.GetComponent<Canvas>();
        Debug.Log("Camera: " + MainScript.GetInstance().LoaderInstance.LoaderCam);
        Debug.Log("Set for canvas: " + uiCanvas.gameObject);
        uiCanvas.worldCamera = MainScript.GetInstance().LoaderInstance.LoaderCam;
    }
}
