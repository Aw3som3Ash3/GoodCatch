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
        public int minLevel, maxLevel;
        [SerializeField]
        public int weight;
    }

    [SerializeField]
    Chance[] daySpawn,nightSpawn;


    
    public FishMonster GetRandomFish()
    {
        int totalWeight = 0;
        var monsters = GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Day) ? daySpawn : nightSpawn;
        foreach (var monster in monsters)
        {
            totalWeight += monster.weight;
        }

        //FishMonsterType fishMonster = monsters[0].monster;
        Chance monsterChance = monsters[0];
        int num=UnityEngine.Random.Range(0, totalWeight);
        int cumalativeWeight=0;
        for(int i = 0;i<monsters.Length;i++)
        {
            if (num >cumalativeWeight && num < monsters[i].weight + cumalativeWeight)
            {
                monsterChance = monsters[i];
            }
            cumalativeWeight += monsters[i].weight;
        }
        return monsterChance.monster.GenerateMonster(UnityEngine.Random.Range(monsterChance.minLevel, monsterChance.maxLevel));
    }
}
