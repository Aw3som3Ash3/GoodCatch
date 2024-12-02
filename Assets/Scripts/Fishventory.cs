using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Fishventory
{

    [SerializeField]
    List<FishMonster> fishies = new List<FishMonster>();
    [SerializeField]
    int maxSize;
    public IReadOnlyList<FishMonster> Fishies { get { return fishies; } }
    public Fishventory(int maxSize=int.MaxValue)
    {
        this.maxSize = maxSize;
    }
    /// <summary>
    /// adds fish to fishventory, returns 0 if full, returns 1 if successful, returns -1 if repeat
    /// </summary>
    /// <param name="fish"></param>
    /// <returns></returns>
    public int AddFish(FishMonster fish)
    {
        
        if (fishies.Count >= maxSize)
        {
            return 0;
        }

        if (!fishies.Contains(fish))
        {
            fishies.Add(fish);
            return 1;
        }
        else
        {
            return -1;
        }

    }

    public void RemoveFish(FishMonster fish)
    {
        fishies.Remove(fish);
    }
    public void RestoreHealthAllFish()
    {
        foreach (var fish in fishies)
        {
            fish.RestoreAllHealth();
        }
    }

    public FishMonster SwapFish(int index,FishMonster fishMonster)
    {
        var temp = fishies[index];
        fishies[index] = fishMonster;
        return temp;

    }
}
