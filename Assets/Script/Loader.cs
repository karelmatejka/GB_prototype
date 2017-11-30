using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Loader : MonoBehaviour {

    public Camera LoaderCam;
    public GameObject EventSystemPrefab;

    [System.Serializable]
    public struct MenuStruct
    {
        public MenuInit MenuPrefab;
        [HideInInspector] public MenuInit MenuInstance;
    }
    public MenuStruct[] MenuField;

    [HideInInspector] public int activemenu = -1;

    public string activelevel;
    bool menuclicked = false;

    // Use this for initialization
    void Start()
    {
        LevelDefinition test;
        GameObject go;

        MainScript.GetInstance();

        if (GameObject.Find("EventSystem") == null)
        {
            go = Instantiate(EventSystemPrefab);
            go.transform.SetParent(this.transform);
        }

        test = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;

        if (test == null)
        {
            InitMenu();
        }

    }

    // Update is called once per frame

    public void InitMenu()
    {
        OpenMenu(0);
    }
    
    void Update()
    {

    }

    public void OpenMenu(int menuID)
    {
        Debug.Log("Opening menu: " + menuID);
        MenuField[menuID].MenuInstance = MenuInit.Instantiate(MenuField[menuID].MenuPrefab);
        MenuField[menuID].MenuInstance.transform.SetParent(this.transform);
        StartCoroutine(EnableMenu(menuID, true));
        
    }

    public void CloseMenu(int menuID)
    {
        StartCoroutine(EnableMenu(menuID, false));
        if (MenuField[menuID].MenuInstance != null)
        {
            Destroy(MenuField[menuID].MenuInstance.gameObject);
        }
        activemenu = -1;
    }

    public IEnumerator EnableMenu(int menuID, bool enable)
    {
        int i;

        if (MenuField[menuID].MenuInstance != null)
        {
            for (i = 0; i < MenuField[menuID].MenuInstance.buttons.Length; i++)
            {
                int tempInt = i;
                if (enable)
                {
                    MenuField[menuID].MenuInstance.buttons[i].onClick.AddListener(() => ButtonClicked(tempInt));
                    MenuField[menuID].MenuInstance.buttons[i].interactable = true;
                    Debug.Log("Button added to menu: " + menuID + " ID: " + i);
                }
                else
                {
                    MenuField[menuID].MenuInstance.buttons[i].onClick.RemoveListener(() => ButtonClicked(tempInt));
                    MenuField[menuID].MenuInstance.buttons[i].interactable = false;
                    Debug.Log("Button removed from menu: " + menuID + " ID: " + i);
                }
            }

            if (enable)
            {
                if (MenuField[menuID].MenuInstance.previousbutton == null)
                {
                    MenuField[menuID].MenuInstance.firstbutton.Select();
                }
                else
                {
                    MenuField[menuID].MenuInstance.previousbutton.Select();
                }
                activemenu = menuID;
                Debug.Log("Menu ID displayed: " + activemenu);
            }
            else
            {
                Debug.Log("Menu ID hidden: " + activemenu);
                activemenu = -1;
            }

            yield return null;
            if (enable)
            {
                menuclicked = false;
            }
        }
    }

    public void ButtonClicked(int buttonpressed)
    {
        Debug.Log("ButtonPressed in menu: " + activemenu + " ID: " + buttonpressed);
        if (!menuclicked)
        {
            MenuField[activemenu].MenuInstance.previousbutton = MenuField[activemenu].MenuInstance.buttons[buttonpressed];
            menuclicked = true;
            if (activemenu == 0) //Main menu
            {
                if (buttonpressed == 0) //Start
                {
                    LoadLevel("Scenes/Prototype");
                }
                else if (buttonpressed == 1) //Start
                {
                    LoadLevel("Scenes/LevelTest02");
                }
                else if (buttonpressed == 2) //Config
                {
                    menuclicked = false;
                }
                else if (buttonpressed == 3)  //Exit
                {
                    StartCoroutine(EnableMenu(0, false));
                    OpenMenu(2);
                }
            }
            else if (activemenu == 1) //Pause menu
            {
                if (buttonpressed == 0) //Yes
                {
                    MainScript.GetInstance().InitLevel(false);
                }
                else if (buttonpressed == 1) //No
                {
                    ResumeGame();
                }
            }
            else if (activemenu == 2) //Confirm Exit Game
            {
                if (buttonpressed == 0) //Yes
                {
                    menuclicked = false;
                    QuitGame();                    
                }
                else if (buttonpressed == 1) //No
                {
                    CloseMenu(2);
                    StartCoroutine(EnableMenu(0, true));
                }
            }
        }
    }

    public void ResumeGame()
    {
        CloseMenu(1);
        Time.timeScale = 1.0f;
        int i;
        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Length; i++)
        {
            MainScript.GetInstance().PlayersToFollow[i].GoingFromMenu = true;
            Debug.Log("GoingFromMenu");
        }
    }

    public void LoadLevel(string level)
    {
        StartCoroutine(LoadLevelCorutine(level, true));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator UnLoadLevel()
    {
        StartCoroutine(EnableMenu(1, false));
        MainScript.GetInstance().GuiInstance.Fade(true);

        while (MainScript.GetInstance().GuiInstance.fading)
        {
            yield return null;
        }

        AsyncOperation async;
        Debug.Log("Unloading the level: " + activelevel);
        LoaderCam.transform.SetParent(this.transform);
        LoaderCam.transform.localPosition = Vector3.zero;

        async = SceneManager.UnloadSceneAsync(activelevel);

      
        while (!async.isDone)
        {
            yield return null;
        }
        ResumeGame();
        OpenMenu(0);
        MainScript.GetInstance().GuiInstance.Fade(false);
    }

    public IEnumerator LoadLevelCorutine(string level, bool add)
    {
        AsyncOperation async;

        StartCoroutine(EnableMenu(0, false));

        MainScript.GetInstance().GuiInstance.Fade(true);

        while (MainScript.GetInstance().GuiInstance.fading)
        {
            yield return null;
        }

        CloseMenu(0);

        if (add)
        {
            async = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        }
        else
        {
            async = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        }
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            var scaledPerc = 0.5f * async.progress / 0.9f;
            //StatusText.text = "<Loading Map : " + LevelInfo.LevelName + " : " + (100f * scaledPerc).ToString("F0") + ">";
        }

        async.allowSceneActivation = true;
        float perc = 0.5f;
        while (!async.isDone)
        {
            yield return null;
            perc = Mathf.Lerp(perc, 1f, 0.05f);
            //StatusText.text = "<Loading Map : " + LevelInfo.LevelName + " : " + (100f * perc).ToString("F0") + ">";
        }

        //StatusText.text = "<Loading Complete : " + LevelInfo.LevelName + " : 100>";
        LoaderCam.transform.SetParent(Camera.main.transform);
        LoaderCam.transform.localPosition = Vector3.back * 150;
        activelevel = level;
        MainScript.GetInstance().InitLevel(true);
        MainScript.GetInstance().GuiInstance.Fade(false);
    }


}
