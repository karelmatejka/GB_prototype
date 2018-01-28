using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerEnvelope" && MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].Goal && !MainScript.GetInstance().Cutscene)
        {
            MainScript.GetInstance().FinishLevel(other.gameObject.GetComponent<Player>());
            Destroy(this.gameObject);
        }
    }
}
