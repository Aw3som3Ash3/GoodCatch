using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEffect : StatusEffect
{
    [SerializeField]
    [Range(0, 1)]
    float damageTransfer;

    
    public override void DoEffect(CombatManager.Turn turn)
    {
        
    }

    public float TransferDamage( float damage,Element element, Ability.AbilityType type,StatusEffectInstance effectInstance)
    {

        float damageToTransfer= damage*damageTransfer;
        Element.Effectiveness effectiveness;
        effectInstance.owner.TakeDamage(damage, element, type,out effectiveness);

        return damage-damageToTransfer;
    }

    
}
