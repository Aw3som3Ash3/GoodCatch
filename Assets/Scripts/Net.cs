using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Net", menuName = "Items/Net", order = 3)]
public class Net : CombatItem
{
    [SerializeField]
    [Range(0,1)]
    float catchBonus;
    public float CatchBonus { get { return catchBonus; } }
   
}
