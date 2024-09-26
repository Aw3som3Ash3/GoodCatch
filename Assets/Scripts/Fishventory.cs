using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Fishventory 
{
    [SerializeField]
    [SerializeReference]
    List<FishMonster> fishies=new List<FishMonster>();
    [SerializeField]
    int maxSize;
    public IReadOnlyList<FishMonster> Fishies { get { return fishies; } }

    public bool AddFish(FishMonster fish)
    {
        if (fishies.Count > maxSize)
        {
            return false;
        }
       
        if (!fishies.Contains(fish))
        {
            fishies.Add(fish);
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void RemoveFish(FishMonster fish)
    {
        fishies.Remove(fish);
    }
}
