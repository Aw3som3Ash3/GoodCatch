using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishZone : SaveableObject,ISaveable
{
    [SerializeField]
    SpawnTables spawnTable;

    [SerializeField]
    int minAmount, maxAmount;

    [Serializable]
    struct Data
    {
        [SerializeField]
        public int amount;
    }
    [SerializeField]
    Data data;

    public override object DataToSave => data;

    // Start is called before the first frame update
    void Start()
    {
        data.amount = UnityEngine.Random.Range(minAmount, maxAmount + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FishMonsterType GetRandomFish( Action fishingSucceeded)
    {
        if (data.amount < 0)
        {
            return null;
        }
       
        fishingSucceeded += () => data.amount--;
        return spawnTable.GetRandomFish();
        
    }

    public override void Load(string json)
    {
        //JsonUtility.FromJsonOverwrite(json, this);
        var data = JsonUtility.FromJson<Data>(json);
        this.data = data;

    }
}
