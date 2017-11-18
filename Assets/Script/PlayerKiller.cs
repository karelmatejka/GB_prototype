using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKiller : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject blood;
        if (coll.tag == "PlayerEnvelope")
        {
            Debug.Log("Killed entity:" + coll.tag);
            blood = MainScript.GetInstance().InstantiateObject(MainScript.GetInstance().BloodInstance, coll.transform.position);
            Destroy(blood.gameObject, 1);
            MainScript.GetInstance().RestartLevel(coll.gameObject.GetComponent<Player>());
        }
    }
}
