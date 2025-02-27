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
    public Sprite Icon =>Icon;
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
    public bool UseAbility(CombatManager.Turn user, CombatManager.CombatDepth targetDepth)
    {
        if (targetDepth == null)
        {
            return false;
        }
        CombatManager.Team targetedTeam=0;
        if (user.team == CombatManager.Team.player)
        {
            if (targetTeam == TargetTeam.enemy)
            {
                targetedTeam=CombatManager.Team.enemy;
            }
            else
            {
                targetedTeam = CombatManager.Team.player;
            }

        }else if (user.team == CombatManager.Team.enemy)
        {
            if (targetTeam == TargetTeam.enemy)
            {
                targetedTeam = CombatManager.Team.player;
            }
            else
            {
                targetedTeam = CombatManager.Team.enemy;
            }

        }
        var targets = targetDepth.TargetSide(targetedTeam);

        if (UnityEngine.Random.Range(0, 1) - ((user.accuracy - (targetTeam == TargetTeam.enemy ? targets[0].dodge : 0)) * 0.01) < accuracy)
        {
            float damageMod = damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);
            
            if(targetTeam == TargetTeam.enemy)
            {
                float propigatedDamage = baseDamage + damageMod;
                for (int i = 0; i < targets.Count; i++)
                {

                    if (targets[i].effects.Count > 0)
                    {
                        foreach (var effectInstance in targets[i].effects.Where((x) => x is DefensiveEffect.DefensiveEffectInstance))
                        {
                            propigatedDamage = (effectInstance as DefensiveEffect.DefensiveEffectInstance).MitigateDamage(propigatedDamage, element, abilityType, effectInstance);
                        }
                    }
                    if (!piercing)
                    {
                        propigatedDamage = targets[i].TakeDamage(propigatedDamage, element, abilityType) / 2;
                    }
                    else
                    {
                        targets[i].TakeDamage(propigatedDamage, element, abilityType);
                    }

                    foreach (var effect in targets[i].effects.Where((x) => x.effect is ThornEffect))
                    {
                        (effect.effect as ThornEffect).ReflectDamage(user);
                    }

                    ProctEffectHostile(user, targets[i],Mathf.Pow(0.5f,i));
                    if (ForcedMovement != 0)
                    {
                        targets[i].ForcedMove(ForcedMovement);
                    }
                }
               
            }
            else if (targetTeam == TargetTeam.friendly)
            {

                if (baseDamage < 0)
                {
                    targets[0].fish.Restore(-baseDamage + damageMod);
                    
                }else if (baseDamage > 0)
                {
                    targets[0].TakeDamage(baseDamage + damageMod, element, abilityType);
                }
                ProctEffectFriendly(user, targets[0]);
                if (ForcedMovement != 0)
                {
                    targets[0].ForcedMove(ForcedMovement);
                }
            }
           

            return true;
        }
        else
        {
            Debug.Log("missed target");
            return false;
        }

        

    }
    public bool UseAbility(CombatManager.Turn user, CombatManager.Turn target, out bool hit, out float damageDone)
    {
        if (target == null)
        {
            hit = false;
            damageDone = 0;
            
            return false;
        }
        if (UnityEngine.Random.Range(0, 1) - ((user.accuracy - (targetTeam == TargetTeam.enemy ? target.dodge : 0)) * 0.01) < accuracy)
        {
            Debug.Log("attacking: " + target);
            float damageMod = damageMultiplier * (abilityType == AbilityType.attack ? user.attack : user.special);



            if (baseDamage > 0)
            {
                float outgoingDamage = baseDamage + damageMod;
                if (target.effects.Count > 0)
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
            else if (baseDamage < 0)
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
                ProctEffectHostile(user, target,1);
            }
            else if (targetTeam == TargetTeam.friendly)
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


        return true;
    }

    void ProctEffectHostile(CombatManager.Turn user, CombatManager.Turn target,float muliplier)
    {
        foreach (var effect in effects)
        {
            float proctBonus = (user.special / 5f) * 0.01f ;
            float targetDef = (target.special / 5f) * 0.01f ;
            if (UnityEngine.Random.Range(0, 1f)  < (effect.Chance + proctBonus) *(target.HadEffectLastTurn(effect.Effect)? 0.15f:1)* muliplier - targetDef)
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


