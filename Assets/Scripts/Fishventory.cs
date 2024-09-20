using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Fishventory 
{
    [SerializeField]
    [SerializeReference]
    List<FishMonster> fishies;
    public IReadOnlyList<FishMonster> Fishies { get { return fishies; } }

    public void AddFish(ref FishMonster fish)
    {
        if (!fishies.Contains(fish))
        {
            fishies.Add(fish);
        }
        
    }

    public void RemoveFish(FishMonster fish)
    {
        fishies.Remove(fish);
    }
}
