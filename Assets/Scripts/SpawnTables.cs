using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnTable", menuName = "Fish Monster/SpawnTable", order = -1)]
public class SpawnTables : ScriptableObject
{
    [Serializable]
    struct Chance
    {
        [SerializeField]
        public FishMonsterType monster;
        [SerializeField]
        public int weight;
    }

    [SerializeField]
    Chance[] monsters;

    
    public FishMonsterType GetRandomFish()
    {
        int totalWeight = 0;
        foreach (var monster in monsters)
        {
            totalWeight += monster.weight;
        }

        FishMonsterType fishMonster = monsters[0].monster;
        int num=UnityEngine.Random.Range(0, totalWeight);
        int cumalativeWeight=0;
        for(int i = 0;i<monsters.Length;i++)
        {
            if (num >cumalativeWeight && num < monsters[i].weight + cumalativeWeight)
            {
                fishMonster = monsters[i].monster;
            }
        }
        return fishMonster;
    }
}
