using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishZone : SaveableObject,ISaveable
{
    [SerializeField]
    SpawnTables daySpawnTable,nightSpawnTable;
    

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
        if (GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Day))
        {
            return daySpawnTable.GetRandomFish();
        }
        else if (GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Night))
        {
            return nightSpawnTable.GetRandomFish();
        }
        return daySpawnTable.GetRandomFish();

    }

    public override void Load(string json)
    {
        //JsonUtility.FromJsonOverwrite(json, this);
        var data = JsonUtility.FromJson<Data>(json);
        this.data = data;

    }

    private void OnDrawGizmos()
    {
        var col =this.GetComponent<Collider>();
        Gizmos.color = new Color(0, 1, 0, 0.5f);
       
        Gizmos.DrawCube(col.bounds.center,new Vector3(col.bounds.size.x, col.bounds.size.y, col.bounds.size.z));
    }
}
