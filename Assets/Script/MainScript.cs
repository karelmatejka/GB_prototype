using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class MainScript : MonoBehaviour {

    [HideInInspector] public List<Player> PlayersToFollow;
    [HideInInspector] public LevelDefinition levelDefinitionScript;
    public GameObject BloodInstance;
    public AudioSource[] ButtonSelectSound;
    public Player[] PlayerPrefab;


    [HideInInspector] public Loader LoaderInstance = null;

    [HideInInspector] public bool LevelLoaded = false;

    public GUInterface GuiPrefab;
    [HideInInspector] public GUInterface GuiInstance;

    public InputDevice joystick;

    [HideInInspector] public float GlobalSoundVolume = 1;

    //-------------SAVE-------------------
    public int Coins = 0;
    //------------------------------------


    static MainScript instance = null;
    public static MainScript GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Instantiate(Resources.Load("MainScript", typeof(MainScript))) as MainScript;
            instance.InitMainScript();
        }
        return instance;
    }

    // Use this for initialization
    void InitMainScript()
    {
        levelDefinitionScript = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;
        GuiInstance = Instantiate(GuiPrefab) as GUInterface;

        LoaderInstance = GameObject.FindObjectOfType(typeof(Loader)) as Loader;

        if (LoaderInstance == null)
        {
            StartCoroutine(LoadMenu());
            Debug.Log("Starting Level Without Menu");
        }

        
        int i;
        
        /*for (i = 0; i < InputManager.Devices.Count; i++)
        {
            Debug.Log("Joystick Found: " + InputManager.Devices[i].Name);
        }
        joystick = InputManager.ActiveDevice;*/
    }

    public IEnumerator LoadMenu()
    {
        AsyncOperation async;

        async = SceneManager.LoadSceneAsync("Scenes/MainScene", LoadSceneMode.Additive);

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }

        MainScript.GetInstance().LoaderInstance = GameObject.FindObjectOfType(typeof(Loader)) as Loader;
        Debug.Log("Loader found: " + MainScript.GetInstance().LoaderInstance);
        MainScript.GetInstance().LoaderInstance.InitMenu();
        MainScript.GetInstance().LoaderInstance.CloseMenu(0);
        MainScript.GetInstance().LoaderInstance.ActiveLevel = SceneManager.GetActiveScene().name;
        MainScript.GetInstance().transform.SetParent(MainScript.GetInstance().LoaderInstance.transform);
        InitLevel(true);
    }


    public void RestartLevel(Player player)
    {
        player.transform.position = levelDefinitionScript.StartingPosition.transform.position - Vector3.right * 1 + Vector3.right * 2 * (player.ControllerIndex - 1);
        Debug.Log("Player Starting Position: " + player.transform.position);
        player.InitPlayer(false);
    }

    public GameObject InstantiateObject(GameObject prefabObject, Vector3 position, Quaternion rotation)
    {
        GameObject go;
        go = Instantiate(prefabObject, position, rotation) as GameObject;
        go.transform.SetParent(levelDefinitionScript.transform);
        go.transform.localScale = prefabObject.transform.localScale;
        return go;

    }

    public void InitLevel(bool init)
    {   
        if (init)
        {
            levelDefinitionScript = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;

            levelDefinitionScript.InitMap();

            /*-----INIT PLAYERS------*/
            PlayersToFollow = new List<Player>();

            PlayersToFollow.Add(Instantiate(PlayerPrefab[0]) as Player);
            RestartLevel(PlayersToFollow[0]);

            if (LoaderInstance != null)
            {
                GuiInstance.transform.SetParent(LoaderInstance.transform);
            }

            GuiInstance.InitGui();
            LevelLoaded = true;
        }
        else
        {
            LevelLoaded = false;

            PlayersToFollow[0].gameObject.SetActive(false);

            if (LoaderInstance != null)
            {
                StartCoroutine(LoaderInstance.UnLoadLevel());
            }
        }
    }

    public void PlayRandomSound(AudioSource[] audiofield, Vector3 where, bool is3d)
    {
        int i;
        float volume = 0;
        float distance;
        int r;
        AudioSource aSource;
        r = Random.Range(0, audiofield.Length);

        if (!is3d)
        {
            volume = 1;
        }
        else
        {
            for (i = 0; i < PlayersToFollow.Count; i++)
            {
                distance = (where - PlayersToFollow[i].transform.position).magnitude;
                volume = volume + 40 - distance;
            }
        }
        if (volume > 0)
        {
            aSource = Instantiate(audiofield[r]) as AudioSource;
            if (is3d) aSource.volume = aSource.volume * volume / 40 * GlobalSoundVolume;
            Destroy(aSource.gameObject, aSource.clip.length);
        }
        
    }


}
