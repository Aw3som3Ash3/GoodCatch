using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Fish Monster/Ability", order = 1)]
public class Ability : ScriptableObject,ISerializationCallbackReceiver
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
        self,
        all
    }
    public enum TargetingType
    {
        single,
        all,
    }
    public enum AbilityType
    {
        attack,
        special,
    }
    [SerializeField]
    public bool ignoreSelf=false;
    [Serializable]
    public struct EffectChance
    {
        [SerializeField]
        StatusEffect statusEffect;
        public StatusEffect Effect { get { return statusEffect; } }
        [SerializeField]
        [Range(0, 1)]
        float effectChance;
        public float Chance { get { return effectChance; } }
    }
    [SerializeField]
    AbilityType abilityType;
    [SerializeField]
    Depth availableDepths, targetableDepths;
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
    public bool Piercing { get; }
    [SerializeField]
    int staminaUsage;
    public int StaminaUsage { get { return staminaUsage; } }
    [SerializeField]
    int baseDamage;

    [SerializeField]
    [Range(0, 1)]
    float accuracy;
    public float Accuracy { get { return accuracy; } }
    [SerializeField]
    float damageMultiplier;

    [SerializeField]
    Element element;
    public Element Element { get { return element; } }
    [SerializeField]
    EffectChance[] effects;
    public EffectChance[] Effects { get { return effects; } }
    [SerializeField]
    [Range(-2, 2)]
    int forcedMovement = 0;
    public int ForcedMovement { get { return forcedMovement; } }

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get; private set; }
    [SerializeField]
    ParticleSystem abilityVFX, targetVFX;
    public ParticleSystem AbilityVFX { get {  return abilityVFX; } }
    public ParticleSystem TargetVFX { get { return targetVFX; } }
    //List<FishMonster> targets;

    [SerializeField]
    string abilityID;
    public string AbilityID { get { return abilityID; } }
    public static Dictionary<string, Ability> getAbilityById=new();

    public bool DepthTargetable(Depth depth)
    {
        return targetableDepths.HasFlag(depth);
    }
    public bool CanUse(Depth depth)
    {
        return availableDepths.HasFlag(depth);
    }
    public float GetDamage(CombatManager.Turn user)
    {
        float damage=0;
        if (baseDamage>0)
        {
            damage= baseDamage+ damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);
        }
        else if(baseDamage<0)
        {
            damage = -baseDamage + damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);
        }

        return damage;
    }
    public float GetDamage(FishMonster fish)
    {
        float damage = 0;
        if (baseDamage > 0)
        {
            damage = baseDamage + damageMultiplier * (abilityType == AbilityType.attack ? fish.Attack.value : fish.Special.value);
        }
        else if (baseDamage < 0)
        {
            damage = -baseDamage + damageMultiplier * (abilityType == AbilityType.attack ? fish.Attack.value : fish.Special.value);
        }

        return damage;
    }
    public float GetEffectBonus(FishMonster fish)
    {
        float proctBonus = (fish.Special.value / 5) * 0.01f;
        
        return proctBonus;
    }
    public bool UseAbility(CombatManager.Turn user, CombatManager.Turn target, out bool hit, out float damageDone)
    {
        if (target == null)
        {
            hit = false;
            damageDone = 0;
            
            return false;
        }
        if (baseDamage < 0)
        {
            target.fish.Restore(health: -baseDamage);
            damageDone = baseDamage;
            
            hit = true;
        }
        else
        {
            if (UnityEngine.Random.Range(0, 1) - ((user.accuracy -  (baseDamage >= 0 ? target.dodge:0 ) ) * 0.01) < accuracy)
            {
                Debug.Log("attacking: " + target);
                float damageMod = damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);



                if (baseDamage > 0)
                {
                    float outgoingDamage = baseDamage + damageMod;
                    if (target.effects.Count>0)
                    {
                        foreach (var effectInstance in target.effects.Where((x) => x is DefensiveEffect.DefensiveEffectInstance))
                        {
                            outgoingDamage = (effectInstance as DefensiveEffect.DefensiveEffectInstance).MitigateDamage(outgoingDamage, element, abilityType, effectInstance);
                        }
                    }
                    //Element.Effectiveness effectivenss;
                    
                    damageDone = target.TakeDamage(outgoingDamage, element, abilityType);

                    foreach (var effect in target.effects.Where((x) => x.effect is ThornEffect))
                    {
                        (effect.effect as ThornEffect).ReflectDamage(user);
                    }
                    
                }
                else if(baseDamage<0)
                {
                    target.fish.Restore(-baseDamage + damageMod);
                    damageDone = baseDamage;
                    
                }
                else
                {
                    damageDone = 0;
                    
                }

                if (targetTeam == TargetTeam.enemy)
                {
                    ProctEffectHostile(user, target);
                }else if (targetTeam == TargetTeam.friendly)
                {
                    ProctEffectFriendly(user, target);
                }

                hit = true;
            }
            else
            {
                Debug.Log("missed: " + target);
                damageDone = 0;
                
                hit = false;
            }
        }


        return true;
    }

    void ProctEffectHostile(CombatManager.Turn user, CombatManager.Turn target)
    {
        foreach (var effect in effects)
        {
            float proctBonus = (user.special / 5) * 0.01f ;
            float targetDef = (target.special / 5) * 0.01f ;
            if (UnityEngine.Random.Range(0, 1) - proctBonus < (effect.Chance)*(target.HadEffectLastTurn(effect.Effect)? 0.15f:1)- targetDef)
            {
                target.AddEffects(effect.Effect,user);
            }
        }
    }
    void ProctEffectFriendly(CombatManager.Turn user, CombatManager.Turn target)
    {
        foreach (var effect in effects)
        {
            float proctBonus = (user.special / 5) * 0.01f;
            if (UnityEngine.Random.Range(0, 1) - proctBonus < (effect.Chance))
            {
                target.AddEffects(effect.Effect, user);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        if (abilityID == null ||abilityID=="" || (getAbilityById.ContainsKey(abilityID) && getAbilityById[abilityID] != this))
        {
            abilityID = Guid.NewGuid().ToString();
            
        }
        getAbilityById[abilityID] = this;
    }

}


