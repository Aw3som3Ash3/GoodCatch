using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook", menuName = "Items/Fish Hook", order = 3)]
public class CombatHook : CombatItem
{
    [SerializeField]
    [Range(0,2)]
    float catchBonus;
    public float CatchBonus { get { return catchBonus; } }
   
}
