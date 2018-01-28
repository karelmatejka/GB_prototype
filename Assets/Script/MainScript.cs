using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class MainScript : MonoBehaviour {

    [HideInInspector] public List<Player> PlayersToFollow;
    //[HideInInspector] public LevelDefinition levelDefinitionScript;
    public GameObject BloodInstance;
    public GameObject CoinPrefab;
    public GameObject FuelPrefab;
    public AudioSource[] ButtonSelectSound;
    public Player[] PlayerPrefab;
    public GameObject MoveToPositionTrigger;
    [HideInInspector] public FlyingSaucer FlyingSaucerInstance;

    [HideInInspector] public Loader LoaderInstance = null;

    [HideInInspector] public bool Cutscene = true;

    public GUInterface GuiPrefab;
    [HideInInspector] public GUInterface GuiInstance;

    public InputDevice joystick;

    [HideInInspector] public float GlobalSoundVolume = 1;

    [HideInInspector] public CameraArea CameraAreaScript;

    //-------------SAVE-------------------
    public int AllCoins = 0;
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
        //levelDefinitionScript = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;
        CameraAreaScript = GameObject.FindObjectOfType(typeof(CameraArea)) as CameraArea;

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
        int i;

        async = SceneManager.LoadSceneAsync("Scenes/MainScene", LoadSceneMode.Additive);

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }

        LoaderInstance = GameObject.FindObjectOfType(typeof(Loader)) as Loader;
        Debug.Log("Loader found: " + MainScript.GetInstance().LoaderInstance);
        LoaderInstance.InitMenu();
        LoaderInstance.CloseMenu(0);
        LoaderInstance.ActiveLevel = SceneManager.GetActiveScene().name;

        for (i = 0; i < LoaderInstance.LevelNames.Count; i++)
        {
            if (LoaderInstance.LevelNames[i] == LoaderInstance.ActiveLevel)
            {
                LoaderInstance.ActiveLevelId = i;
            }
        }


        this.transform.SetParent(MainScript.GetInstance().LoaderInstance.transform);
        InitLevel(true, false);
    }

    public IEnumerator MoveCamera(Vector3 end)
    {
        Vector3 velocity = Vector3.zero;
        while (Vector3.Distance(Camera.main.transform.position, end) > 0.1f)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, end, ref velocity, 0.3f);

            yield return null;
        }
    }

    public IEnumerator DeathAnimation(Player player)
    {
        
        int i;
        GameObject blood;
        Cutscene = true;
        bool goal;

        goal = LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].Goal;

        blood = MainScript.GetInstance().InstantiateObject(MainScript.GetInstance().BloodInstance, player.transform.position, Quaternion.identity);
        MainScript.GetInstance().PlayRandomSound(player.KillSounds, this.transform.position, false);
        Destroy(blood.gameObject, 1);
        player.PlayerBody.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        SetPlayerPos(player, true);

        StartCoroutine(MoveCamera(CameraAreaScript.GetCameraPos()));

        yield return new WaitForSeconds((CameraAreaScript.GetCameraPos() - Camera.main.transform.position).magnitude * 0.01f + 0.05f);

        LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].ReappearFuel();
        FlyingSaucerInstance.ResetFuelIndicators(LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].FuelToCollect);

        //yield return new WaitForSeconds(0.1f);

        if (goal)
        {
            FlyingSaucerInstance.CloseDoor();
            yield return new WaitForSeconds(0.1f);
        }

        player.PlayerBody.gameObject.SetActive(true);
        player.SetJump(false);

        

        for (i = 0; i < 5; i++)
        {
            player.PlayerBody.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            player.PlayerBody.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        Cutscene = false;
        yield return null;
    }

    public void RestartLevel(Player player)
    {
        if (!Cutscene)
        {
            StartCoroutine(DeathAnimation(player));
        }
    }

    public void FinishLevel(Player player)
    {
        StartCoroutine(FlyingSaucerInstance.FlyAwayAnimation());
    }

    public Vector3 GetPlayerStartPos (Player player, bool death)
    {
        float deathShift = 0;
        if (death)
        {
            deathShift = 1.05f;
        }
        return LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].StartingPosition.transform.position - Vector3.right * 1 + Vector3.right * 2 * (player.ControllerIndex - 1) - Vector3.right * deathShift;
    }

    public void SetPlayerPos(Player player, bool death)
    {
        
        player.transform.position = GetPlayerStartPos(player, death);
        //Debug.Log("Player Starting Position: " + player.transform.position);
        player.transform.SetParent(CameraAreaScript.transform);
        player.InitPlayer(false);
    }

    public GameObject InstantiateObject(GameObject prefabObject, Vector3 position, Quaternion rotation)
    {
        GameObject go;
        go = Instantiate(prefabObject, position, rotation) as GameObject;
        go.transform.SetParent(CameraAreaScript.transform);
        go.transform.localScale = prefabObject.transform.localScale;
        return go;

    }

    void FixedUpdate()
    {
        if (Cutscene)
        {
            return;
        }
        Vector3 velocity = Vector3.zero;
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, CameraAreaScript.GetCameraPos(), ref velocity, 0.1f);
    }

    public void InitLevel(bool init, bool openMenu)
    {   
        if (init)
        {
            Cutscene = true;

            GuiInstance.InitGui();

            CameraAreaScript = GameObject.FindObjectOfType(typeof(CameraArea)) as CameraArea;

            //levelDefinitionScript = LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId];

            LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].DisplayMap(true);

            /*-----INIT PLAYERS------*/
            PlayersToFollow = new List<Player>();

            PlayersToFollow.Add(Instantiate(PlayerPrefab[0]) as Player);
            SetPlayerPos(PlayersToFollow[0], false);

            FlyingSaucerInstance = GameObject.Instantiate(Resources.Load("FlyingSaucerGroup", typeof(FlyingSaucer))) as FlyingSaucer;
            FlyingSaucerInstance.ShowFuelIndicators(LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].FuelToCollect);
            FlyingSaucerInstance.Landing(LoaderInstance.LevelDefinitions[LoaderInstance.ActiveLevelId].StartingPosition.transform.position);

            


            if (LoaderInstance != null)
            {
                GuiInstance.transform.SetParent(LoaderInstance.transform);
            }

            GuiInstance.InitGui();
        }
        else
        {
            Cutscene = true;
            PlayersToFollow.Clear();

            if (LoaderInstance != null)
            {
                StartCoroutine(LoaderInstance.UnLoadLevel(openMenu));
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
                volume = volume + 20 - distance;
            }
        }
        if (volume > 0)
        {
            aSource = Instantiate(audiofield[r]) as AudioSource;
            if (is3d) aSource.volume = aSource.volume * volume / 20 * GlobalSoundVolume;
            Destroy(aSource.gameObject, aSource.clip.length);
        }
        
    }


}
