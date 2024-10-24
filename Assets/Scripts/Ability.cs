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
    [Serializable]
    public struct EffectChance
    {
        [SerializeField]
        StatusEffect statusEffect;
        public StatusEffect Effect { get { return statusEffect; } }
        [SerializeField]
        [Range(0,1)]
        float effectChance;
        public float Chance { get { return effectChance; } }
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
    Element element;
    [SerializeField]
    EffectChance[] effects;
    [SerializeField]
    [Range(-2,2)]
    int forcedMovement=0;

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
    public bool UseAbility(FishMonster user,CombatManager.Turn target, out bool hit)
    {
        if (target == null)
        {
            hit = false;
            return false;
        }
        if (UnityEngine.Random.Range(0, 1)+ ((user.accuracy - target.fish.dodge)* 0.01)< accuracy)
        {
            Debug.Log("attacking: " + target);
            float damageMod = damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);
            target.fish.TakeDamage(baseDamage + damageMod, element, abilityType);
            target.ForcedMove(forcedMovement);
            ProctEffect(user,target);
            hit = true;
        }
        else
        {
            Debug.Log("missed: " + target);
            hit = false;
        }
        
        return true;
    }

    void ProctEffect(FishMonster user,CombatManager.Turn target)
    {
        foreach(var effect in effects)
        {
            float proctBonus= (user.special / 5)*0.01f;
            if (UnityEngine.Random.Range(0, 1)+ proctBonus < (effect.Chance))
            {
                target.AddEffects(effect.Effect);
            }
        }
        
    }
}


