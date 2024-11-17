using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishZone : MonoBehaviour
{
    [SerializeField]
    SpawnTables spawnTable;
    [SerializeField]
    int minAmount, maxAmount;
    int amount;

    // Start is called before the first frame update
    void Start()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FishMonsterType GetRandomFish()
    {
        if (amount < 0)
        {
            return null;
        }
        amount--;
        return spawnTable.GetRandomFish();
    }

}
