using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Fish Monster/Ability", order = 1)]
public class Ability:ScriptableObject
{
    enum TargetingType
    {
        singleEnemy,
        multiEnemy,
        self,
        singleTeam,
        multiTeam,
        allEnemy,
        allFish
    }

    [SerializeField]
    Depth availableDepths;
    [SerializeField]
    TargetingType targetingType;
    [SerializeField]
    [Tooltip("only needed for multi")]
    int numOfTarget;
    [SerializeField]
    int maxUsages;
    public int MaxUsages { get { return maxUsages; } }
    [SerializeField]
    int baseDamage;
    [SerializeField]
    StatusEffect statusEffect;
    [SerializeField]
    Element element;

    //List<FishMonster> targets;

    public void UseAbility()
    {

        switch (targetingType)
        {
            case TargetingType.singleEnemy:
                break;
            case TargetingType.multiEnemy:
                break;
            case TargetingType.self:
                break;
            case TargetingType.singleTeam:
                break;
            case TargetingType.multiTeam:
                break;
            case TargetingType.allEnemy: 
                break;
            case TargetingType.allFish:
                break;
        }
    }
}
