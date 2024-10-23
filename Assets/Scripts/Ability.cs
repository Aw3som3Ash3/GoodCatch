using System;
using System.Collections;
using System.Collections.Generic;
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
    public enum TargetTeam
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
    public enum AbilityType
    {
        attack,
        special,
    }
    [SerializeField]
    AbilityType abilityType;
    [SerializeField]
    Depth availableDepths,targetableDepths;
    public Depth AvailableDepths { get { return availableDepths; } }

    public Depth TargetableDepths { get { return targetableDepths; } }
    [SerializeField]
    TargetTeam targetTeam;
    public TargetTeam TargetedTeam { get { return targetTeam; } }

    [SerializeField]
    TargetingType targetingType;
    public TargetingType Targeting { get { return targetingType; } }
    [SerializeField]
    bool piercing;
    [SerializeField]
    int staminaUsage;
    public int StaminaUsage { get { return staminaUsage; } }
    [SerializeField]
    int baseDamage;

    [SerializeField]
    [Range(0,1)]
    float accuracy;
    [SerializeField]
    float damageMultiplier;
    [SerializeField]
    StatusEffect statusEffect;
    [SerializeField]
    Element element;
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get; private set; }
    //List<FishMonster> targets;

    
    public bool DepthTargetable(Depth depth)
    {
        return targetableDepths.HasFlag(depth);
    }
    public bool CanUse(Depth depth)
    {
        return availableDepths.HasFlag(depth);
    }
    public bool UseAbility(FishMonster user,FishMonster target, out bool hit)
    {
        if (target == null)
        {
            hit = false;
            return false;
        }
        if (UnityEngine.Random.Range(0, 1)+ (user.accuracy * 0.01) < accuracy)
        {
            Debug.Log("attacking: " + target);
            float damageMod = damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);
            target.TakeDamage(baseDamage + damageMod, element, abilityType);
            hit = true;
        }
        else
        {
            Debug.Log("missed: " + target);
            hit = false;
        }

        return true;
    }
    //public AbilityInstance NewInstance(FishMonster parent)
    //{
    //    return new AbilityInstance(this, parent);
    //}
    //public class AbilityInstance
    //{
    //    public Ability ability { get; private set; }
    //    public FishMonster parent { get; private set; }

    //    public AbilityInstance(Ability ability, FishMonster parent)
    //    {
    //        this.ability = ability;
    //        this.parent = parent;
    //    }

    //    public bool UseAbility(FishMonster target,out bool hit)
    //    {
    //        if (target == null)
    //        {
    //            hit = false;
    //            return false;
    //        }
    //        if (UnityEngine.Random.Range(0, 1) < ability.accuracy)
    //        {
    //            Debug.Log("attacking: " + target);
    //            float damageMod = ability.damageMultiplier * (ability.abilityType == AbilityType.attack ? parent.attack : parent.special);
    //            target.TakeDamage(ability.baseDamage + damageMod, ability.element, ability.abilityType);
    //            hit = true;
    //        }
    //        else
    //        {
    //            Debug.Log("missed: " + target);
    //            hit = false;
    //        }
           
    //        return true;
    //    }
    //}
}
