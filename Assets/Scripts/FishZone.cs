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

    int amount;

    public override object DataToSave => amount;

    // Start is called before the first frame update
    void Start()
    {
        amount = UnityEngine.Random.Range(minAmount, maxAmount + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FishMonsterType GetRandomFish( Action fishingSucceeded)
    {
        if (amount < 0)
        {
            return null;
        }
       
        fishingSucceeded += () => amount--;
        return spawnTable.GetRandomFish();
        
    }

    public override void Load(string json)
    {
        var data = JsonUtility.FromJson<int>(json);
        amount = data;

    }
}
