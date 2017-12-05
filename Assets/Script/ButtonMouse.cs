using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMouse : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button b = this.transform.GetComponent<Button>();
        b.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!MainScript.GetInstance().LoaderInstance.menuclicked)
        {
            MainScript.GetInstance().PlayRandomSound(MainScript.GetInstance().ButtonSelectSound, this.transform.position, false);
        }
    }
}
