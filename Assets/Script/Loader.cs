using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using InControl;

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

    public string ActiveLevel = "";
    public int ActiveLevelId = -1;

    [HideInInspector] public List<string> LevelNames;
    [HideInInspector] public List<LevelDefinition> LevelDefinitions;

    public bool menuclicked = false;

    public InputModuleActionAdapter InputAdapter;

    bool ExitPressed;
    bool CancelPressed;

    // Use this for initialization
    void Start()
    {
        CameraArea test;
        GameObject go;

        InputAdapter = GetComponent<InputModuleActionAdapter>();

        FeedLevelNames();

        FeedLevelData();      

        if (GameObject.Find("EventSystem") == null)
        {
            go = Instantiate(EventSystemPrefab);
            go.transform.SetParent(this.transform);
        }

        test = GameObject.FindObjectOfType(typeof(CameraArea)) as CameraArea;

        if (test == null)
        {
            InitMenu();
        }
    }


    public void InitMenu()
    {
        OpenMenu(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputAdapter.actions.Exit.WasPressed)
        {
            
            if (activemenu == -1) //Open Pause Menu
            {
                OpenMenu(1);
                Time.timeScale = 0.0f;
            }
            else if (activemenu == 1) //Back to Game
            {
                ResumeGame();
            }
           
        }
        else if (InputAdapter.actions.Cancel.WasPressed && activemenu == 1) //Back to Game
        {
            ResumeGame();
        }

        if ((InputAdapter.actions.Cancel.WasPressed || InputAdapter.actions.Exit.WasPressed) && activemenu == 3) //Back to Menu from Level Selector
        {
            menuclicked = true;
            //StartCoroutine(FadeBetweenMenus(3, 0));
            CloseMenu(3);
            OpenMenu(0);
        }
        if (InputAdapter.actions.Cancel.WasPressed && activemenu == 2) //Back From Exit Game 
        {
            menuclicked = true;
            CloseMenu(2);
            StartCoroutine(EnableMenu(0, true));
        }
    }

    public IEnumerator FadeBetweenMenus(int first, int second)
    {
        MainScript.GetInstance().GuiInstance.Fade(true);
        while (MainScript.GetInstance().GuiInstance.fading)
        {
            yield return null;
        }
        CloseMenu(first);
        OpenMenu(second);
        MainScript.GetInstance().GuiInstance.Fade(false);
        yield return null;
    }

    public void OpenMenu(int menuID)
    {
        //Debug.Log("Opening menu: " + menuID);
        MenuField[menuID].MenuInstance = MenuInit.Instantiate(MenuField[menuID].MenuPrefab);
        MenuField[menuID].MenuInstance.transform.SetParent(this.transform);
        StartCoroutine(EnableMenu(menuID, true));
        MainScript.GetInstance().GuiInstance.InitGui();
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
                    //Debug.Log("Button added to menu: " + menuID + " ID: " + i);
                }
                else
                {
                    MenuField[menuID].MenuInstance.buttons[i].onClick.RemoveListener(() => ButtonClicked(tempInt));
                    MenuField[menuID].MenuInstance.buttons[i].interactable = false;
                    //Debug.Log("Button removed from menu: " + menuID + " ID: " + i);
                }
            }

            if (enable)
            {
                if (MenuField[menuID].MenuInstance.previousbutton == null)
                {
                    if (menuID != 3)
                    {
                        MenuField[menuID].MenuInstance.firstbutton.Select();
                    }
                    else if (ActiveLevel == "")
                    {
                        MenuField[menuID].MenuInstance.firstbutton.Select(); //SELECT THE LATEST LEVEL
                    }
                    else
                    {
                        //Debug.Log("Entering Level Selector From Game");
                        for (i = 0; i < LevelNames.Count; i++)
                        {
                            if (LevelNames[i] == ActiveLevel)
                            {
                                MenuField[menuID].MenuInstance.buttons[i].Select();
                            }
                        }
                    }
                }
                else
                {
                    MenuField[menuID].MenuInstance.previousbutton.Select();
                }
                activemenu = menuID;
                //Debug.Log("Menu ID displayed: " + activemenu);
            }
            else
            {
                //Debug.Log("Menu ID hidden: " + activemenu);
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
        //Debug.Log("ButtonPressed in menu: " + activemenu + " ID: " + buttonpressed);
        if (!menuclicked)
        {
            MenuField[activemenu].MenuInstance.previousbutton = MenuField[activemenu].MenuInstance.buttons[buttonpressed];
            menuclicked = true;
            if (activemenu == 0) //Main menu ---------------------
            {
                if (buttonpressed == 0) //Start pressed
                {
                    //StartCoroutine(FadeBetweenMenus(0, 3));
                    CloseMenu(0);
                    OpenMenu(3);
                }
                else if (buttonpressed == 1) //Config pressed
                {
                    menuclicked = false;
                }
                else if (buttonpressed == 2)  //Exit pressed
                {
                    StartCoroutine(EnableMenu(0, false));
                    OpenMenu(2);
                }
            }
            else if (activemenu == 1) //Pause menu ---------------------
            {
                if (buttonpressed == 0) //Yes pressed
                {
                    MainScript.GetInstance().InitLevel(false, true);
                }
                else if (buttonpressed == 1) //No pressed
                {
                    ResumeGame();
                }
            }
            else if (activemenu == 2) //Confirm Exit Game ---------------------
            {
                if (buttonpressed == 0) //Yes pressed
                {
                    menuclicked = false;
                    QuitGame();                    
                }
                else if (buttonpressed == 1) //No pressed
                {
                    CloseMenu(2);
                    StartCoroutine(EnableMenu(0, true));
                }
            }
            else if (activemenu == 3) //Level Selector ---------------------
            {
                int i;
                for (i = 0; i < MenuField[3].MenuInstance.buttons.Length; i++)
                {
                    if (buttonpressed == i)
                    {
                        LoadLevel(i);
                    }
                }
            }
            else if (activemenu == 4) //Reward Screen ---------------------
            {
                if (buttonpressed == 0) //Next Level
                {
                    MainScript.GetInstance().InitLevel(false, false);
                    LoadLevel(ActiveLevelId + 1);
                }
                else if (buttonpressed == 1) //Replay Level
                {
                    MainScript.GetInstance().InitLevel(false, false);
                    LoadLevel(ActiveLevelId);
                }
                else if (buttonpressed == 2) //LevelSelect
                {
                    MainScript.GetInstance().InitLevel(false, true);
                }
            }
        }
    }

    public void ResumeGame()
    {
        CloseMenu(1);
        Time.timeScale = 1.0f;
        int i;
        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Count; i++)
        {
            MainScript.GetInstance().PlayersToFollow[i].GoingFromMenu = true;
            Debug.Log("GoingFromMenu");
        }
        MainScript.GetInstance().GuiInstance.InitGui();
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(LoadLevelCorutine(level, true));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator UnLoadLevel(bool openMenu)
    {
        StartCoroutine(EnableMenu(1, false));
        MainScript.GetInstance().GuiInstance.Fade(true);

        while (MainScript.GetInstance().GuiInstance.fading)
        {
            yield return null;
        }
        CloseMenu(4);

        AsyncOperation async;
        Debug.Log("Unloading the level: " + ActiveLevel);
        LoaderCam.transform.SetParent(this.transform);
        LoaderCam.transform.localPosition = Vector3.zero;

        async = SceneManager.UnloadSceneAsync(ActiveLevel);

      
        while (!async.isDone)
        {
            yield return null;
        }

        LevelDefinitions[ActiveLevelId].DisplayMap(false);

        ResumeGame();
        if (openMenu)
        {
            OpenMenu(3);
        }
        MainScript.GetInstance().GuiInstance.Fade(false);
    }

    public IEnumerator LoadLevelCorutine(int level, bool add)
    {
        AsyncOperation async;

        StartCoroutine(EnableMenu(3, false));

        MainScript.GetInstance().GuiInstance.Fade(true);

        while (MainScript.GetInstance().GuiInstance.fading)
        {
            yield return null;
        }

        CloseMenu(3);
        CloseMenu(4);

        if (add)
        {
            async = SceneManager.LoadSceneAsync(LevelNames[level], LoadSceneMode.Additive);
        }
        else
        {
            async = SceneManager.LoadSceneAsync(LevelNames[level], LoadSceneMode.Single);
        }
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            //var scaledPerc = 0.5f * async.progress / 0.9f;
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
        ActiveLevel = LevelNames[level];
        ActiveLevelId = level;
        MainScript.GetInstance().InitLevel(true, false);
        MainScript.GetInstance().GuiInstance.Fade(false);
    }

    void FeedLevelData()
    {
        int i;
        string str;
        LevelDefinitions = new List<LevelDefinition>();
        for (i = 0; i < LevelNames.Count; i++)
        {
            str = "Levels/" + LevelNames[i];
            //Debug.Log("Map definition: " + str + " loaded");
            LevelDefinitions.Add(GameObject.Instantiate(Resources.Load(str, typeof(LevelDefinition))) as LevelDefinition);
            LevelDefinitions[i].transform.SetParent(this.transform);
        }
    }

    void FeedLevelNames()
    {
        LevelNames = new List<string>();
        LevelNames.Add("Prototype");
        LevelNames.Add("Prototype2");
    }

}
