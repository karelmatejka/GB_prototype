using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDefinition : MonoBehaviour {

    public GameObject StartingPosition;

    [System.Serializable]
    public struct CoinDef
    {
        public Coin Visual;
        public bool Collected;
    }

    [HideInInspector] public List<CoinDef> CoinsDef;

    public float TimeToBeat;
    [HideInInspector]
    public float PlayerTime;

    public void RemoveCoin(int i)
    {
        CoinDef editcoin;
        editcoin = CoinsDef[i];
        editcoin.Collected = true;
        CoinsDef[i] = editcoin;
    }

    void Start()
    {
        InitMap();
    }

    public void InitMap()
    {
        int i;
        Coin[] go;

        CoinDef newcoin;

        CoinsDef = new List<CoinDef>();
        //go = GameObject.FindGameObjectsWithTag("JumpingColliders");
        go = GetComponentsInChildren<Coin>();
        for (i = 0; i < go.Length; i++)
        {
            newcoin.Collected = false;
            newcoin.Visual = go[i];
            newcoin.Visual.gameObject.SetActive(false);
            newcoin.Visual.setID(i);
            CoinsDef.Add(newcoin);
        }

    }

    public void DisplayMap(bool display)
    {
        int i;
        for (i = 0; i < CoinsDef.Count; i++)
        {
            if (!CoinsDef[i].Collected && display)
            {
                CoinsDef[i].Visual.gameObject.SetActive(true);
            }
            else
            {
                CoinsDef[i].Visual.gameObject.SetActive(false);
            }
        }

    }
}
