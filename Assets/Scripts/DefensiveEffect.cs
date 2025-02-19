using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefensiveEffect : StatusEffect
{
    
    public abstract class DefensiveEffectInstance : StatusEffectInstance
    {

        public abstract float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance);
        public DefensiveEffectInstance(StatusEffect effect, CombatManager.Turn owner) : base(effect, owner)
        {

        }
    }
}


