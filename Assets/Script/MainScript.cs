using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public Player[] PlayersToFollow;
    public LevelDefinition levelDefinitionScript;
    public GameObject BloodInstance;

    static MainScript instance = null;
    public static MainScript GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Instantiate(Resources.Load("MainScript", typeof(MainScript))) as MainScript;
            instance.Init();
        }
        return instance;
    }

    // Use this for initialization
    void Init()
    {
        int i;
        GameObject[] go;

        levelDefinitionScript = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;

        go = GameObject.FindGameObjectsWithTag("PlayerEnvelope");
        PlayersToFollow = new Player[go.Length];
        for (i = 0; i < go.Length; i++)
        {
            PlayersToFollow[i] = go[i].GetComponent<Player>();
            PlayersToFollow[i].transform.position = levelDefinitionScript.StartingPosition.transform.position;
        }
    }

    public void RestartLevel(Player player)
    {
        player.transform.position = levelDefinitionScript.StartingPosition.transform.position;
        player.SetJump();
    }

    public GameObject InstantiateObject(GameObject prefabObject, Vector3 position)
    {
        GameObject go;
        go = Instantiate(prefabObject, position, Quaternion.identity) as GameObject;
        return go;

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
