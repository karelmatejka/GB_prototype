using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public Animator anim;
    public int ID;
    public AudioSource[] CollectCoinSounds;
    
    // Use this for initialization
    void Start ()
    {
        float startPoint = Random.Range(0f, 1f);
        anim.Play("CoinAnim", -1, startPoint);
    }

    public void setID(int id)
    {
        ID = id;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerEnvelope")
        {
            MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].RemoveCoin(ID);
            MainScript.GetInstance().GuiInstance.AddCoins(1);
            MainScript.GetInstance().PlayRandomSound(CollectCoinSounds, this.transform.position, false);

            this.gameObject.SetActive(false);
        }
    }

}
