using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour {

    [HideInInspector] public Player[] PlayersToFollow;
    [HideInInspector] public LevelDefinition levelDefinitionScript;
    public GameObject BloodInstance;
    public AudioSource[] ButtonSelectSound;
    public Player[] PlayerPrefab;

    [HideInInspector] public Loader LoaderInstance = null;

    [HideInInspector] public bool LevelLoaded = false;

    public GUInterface GuiPrefab;
    [HideInInspector] public GUInterface GuiInstance;

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
        player.SetJump();
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
        int i;
        
        if (init)
        {
            levelDefinitionScript = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;

            levelDefinitionScript.InitMap();

            /*-----INIT PLAYERS------*/
            PlayersToFollow = new Player[1];
            PlayersToFollow[0] = Instantiate(PlayerPrefab[0]) as Player;
            PlayersToFollow[0].gameObject.SetActive(false);

            PlayersToFollow[0].gameObject.SetActive(true);
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

    public void PlayRandomSound(AudioSource[] audiofield, Vector3 where)
    {
        int r;
        AudioSource aSource;
        r = Random.Range(0, audiofield.Length);
        aSource = Instantiate(audiofield[r]) as AudioSource;
        Destroy(aSource.gameObject, aSource.clip.length);
    }


}
