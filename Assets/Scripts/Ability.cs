using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Fish Monster/Ability", order = 1)]
public class Ability:ScriptableObject
{
    //enum TargetingType
    //{
    //    singleEnemy,
    //    multiEnemy,
    //    self,
    //    singleTeam,
    //    multiTeam,
    //    allEnemy,
    //    allFish
    //}
    enum TargetTeam
    {
        friendly,
        enemy,
        all
    }
    public enum TargetingType
    {
        single,
        all
    }
    [SerializeField]
    Depth availableDepths,targetableDepths;
    [SerializeField]
    TargetTeam targetTeam;
    [SerializeField]
    TargetingType targetingType;
    public TargetingType Targeting { get { return targetingType; } }
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

    public bool UseAbility(FishMonster target)
    {
        Debug.Log("attacking: " + target);
        target.TakeDamage(baseDamage, element);
        return true;
    }
    public bool DepthTargetable(Depth depth)
    {
        return targetableDepths.HasFlag(depth);
    }
    public bool CanUse(Depth depth)
    {
        return availableDepths.HasFlag(depth);
    }
}
