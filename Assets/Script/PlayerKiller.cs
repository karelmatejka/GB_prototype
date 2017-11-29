using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKiller : MonoBehaviour {
    public bool DestroyedOnTouch;
    public float DestroyOnTouchDelay;
    public GameObject DestroyedPrefab;

    void Update()
    {
        DestroyOnTouchDelay -= Time.deltaTime * 1000;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject blood;
        GameObject explosion;
        if (coll.tag == "PlayerEnvelope")
        {
            Debug.Log("Killed entity:" + coll.tag);
            blood = MainScript.GetInstance().InstantiateObject(MainScript.GetInstance().BloodInstance, coll.transform.position, Quaternion.identity);
            Destroy(blood.gameObject, 1);
            MainScript.GetInstance().RestartLevel(coll.gameObject.GetComponent<Player>());
        }
        if (DestroyedOnTouch && coll.tag != "CameraArea" && DestroyOnTouchDelay < 0)
        {
            Debug.Log("Entity hit:" + coll.gameObject);
            explosion = MainScript.GetInstance().InstantiateObject(DestroyedPrefab, this.transform.position, Quaternion.identity);
            Destroy(explosion.gameObject, 1);
            Destroy(this.gameObject);
            
        }
    }
}
