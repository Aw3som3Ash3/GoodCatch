using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
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
    public enum AbilityType
    {
        attack,
        special,
    }
    [SerializeField]
    AbilityType abilityType;
    [SerializeField]
    Depth availableDepths,targetableDepths;
    public Depth TargetableDepths { get { return targetableDepths; } }
    [SerializeField]
    TargetTeam targetTeam;
    [SerializeField]
    TargetingType targetingType;
    public TargetingType Targeting { get { return targetingType; } }
    [SerializeField]
    int staminaUsage;
    public int StaminaUsage { get { return staminaUsage; } }
    [SerializeField]
    int baseDamage;
    [SerializeField]
    float damageMultiplier;
    [SerializeField]
    StatusEffect statusEffect;
    [SerializeField]
    Element element;

    //List<FishMonster> targets;

    
    public bool DepthTargetable(Depth depth)
    {
        return targetableDepths.HasFlag(depth);
    }
    public bool CanUse(Depth depth)
    {
        return availableDepths.HasFlag(depth);
    }
    public AbilityInstance NewInstance(FishMonster parent)
    {
        return new AbilityInstance(this, parent);
    }
    public class AbilityInstance
    {
        public Ability ability { get; private set; }
        public FishMonster parent;

        public AbilityInstance(Ability ability, FishMonster parent)
        {
            this.ability = ability;
            this.parent = parent;
        }

        public bool UseAbility(FishMonster target)
        {
            Debug.Log("attacking: " + target);
            float damageMod = ability.damageMultiplier * (ability.abilityType == AbilityType.attack ? parent.attack : parent.special);
            target.TakeDamage(ability.baseDamage+ damageMod, ability.element, ability.abilityType);
            return true;
        }
    }
}
