using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDefinition : MonoBehaviour {

    public GameObject StartingPosition;

    [HideInInspector] public List<GameObject> FuelsDef;

    [System.Serializable]
    public struct CoinDef
    {
        public Coin Visual;
        public bool Collected;
    }

    [HideInInspector] public List<CoinDef> CoinsDef;
    [HideInInspector] public int CollectedCoins;

    public float TimeToBeat;
    [HideInInspector] public float PlayerTime;

    [HideInInspector] public int FuelToCollect;
    [HideInInspector] public int CollectedFuel;

    [HideInInspector] public bool Goal;

    public void CollectCoin(int i)
    {
        CoinDef editcoin;
        editcoin = CoinsDef[i];
        editcoin.Collected = true;
        CoinsDef[i] = editcoin;
        MainScript.GetInstance().GuiInstance.AddCoins(1);
    }

    void Start()
    {
        InitMap();
    }

    public void InitMap()
    {
        int i;
        GameObject[] goCoin;
        CoinDef newcoin;

        CoinsDef = new List<CoinDef>();
        goCoin = GameObject.FindGameObjectsWithTag("CoinPosition");
        for (i = 0; i < goCoin.Length; i++)
        {
            //Debug.Log("Coin parent name " + goCoin[i].transform.parent.parent.gameObject.name + ", Parent: " + this.name);
            if (goCoin[i].transform.parent.parent.gameObject.name == this.name)
            {
                newcoin.Collected = false;

                GameObject coinInstance = Instantiate(MainScript.GetInstance().CoinPrefab, goCoin[i].transform.position, Quaternion.identity) as GameObject;
                coinInstance.transform.SetParent(goCoin[i].transform);

                newcoin.Visual = coinInstance.GetComponent<Coin>();
                newcoin.Visual.gameObject.SetActive(false);
                CoinsDef.Add(newcoin);
                newcoin.Visual.setID(CoinsDef.Count -1);
                //Debug.Log("Coin found: " + (CoinsDef.Count -1));
            }
        }
    }

    public void CollectFuel(Vector3 startingPosition)
    {
        CollectedFuel += 1;
        MainScript.GetInstance().FlyingSaucerInstance.CollectFuelIndicator(CollectedFuel, FuelToCollect);

        MainScript.GetInstance().GuiInstance.CollectFuelIndicator(CollectedFuel, startingPosition);

        if (CollectedFuel == FuelToCollect)
        {
            Debug.Log("All fuel canisters collected");
            Goal = true;
            MainScript.GetInstance().FlyingSaucerInstance.OpenDoor();
        }
    }

    public void DisplayFuel(bool display)
    {
        int i;
        GameObject[] goFuel;
        GameObject go;

        if (display)
        {
            FuelsDef = new List<GameObject>();
            goFuel = GameObject.FindGameObjectsWithTag("FuelPosition");
            FuelToCollect = 0;
            for (i = 0; i < goFuel.Length; i++)
            {
                //Debug.Log("Fuel position found. Parent: " + goFuel[i].transform.parent.gameObject.name + ", " + this.name);
                if (goFuel[i].transform.parent.gameObject.name == this.name)
                {
                    go = MainScript.GetInstance().InstantiateObject(MainScript.GetInstance().FuelPrefab, goFuel[i].transform.position, Quaternion.identity);
                    go.transform.SetParent(goFuel[i].transform);
                    FuelsDef.Add(go);
                    FuelToCollect += 1;
                }
            }
            Debug.Log("Number of Fuel canisters found in level: " + FuelToCollect);
            CollectedFuel = 0;
        }
        else
        {
            for (i = 0; i < FuelsDef.Count; i++)
            {
                Destroy(FuelsDef[i].gameObject);
            }
            FuelsDef.Clear();
        }
    }

    public void ReappearFuel()
    {
        int i;
        for (i = 0; i < FuelsDef.Count; i++)
        {
            FuelsDef[i].gameObject.SetActive(true);
        }
        CollectedFuel = 0;
        Goal = false;
    }

    public void DisplayMap(bool display)
    {
        int i;
        int foundCoins = 0;
        for (i = 0; i < CoinsDef.Count; i++)
        {
            if (display)
            {
                if (!CoinsDef[i].Collected)
                {
                    CoinsDef[i].Visual.gameObject.SetActive(true);

                    float startPoint = Random.Range(0f, 1f);
                    CoinsDef[i].Visual.anim.Play("CoinAnim", -1, startPoint);

                    foundCoins += 1;
                }
            }
            else
            {
                CoinsDef[i].Visual.gameObject.SetActive(false);
            }
        }
        CollectedCoins = CoinsDef.Count - foundCoins;
        DisplayFuel(display);
        Goal = false;
    }
}
