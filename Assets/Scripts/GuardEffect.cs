using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Guard", menuName = "Status Effect/Defensive Effects/Guard", order = 2)]
public class GuardEffect : DefensiveEffect
{
    [SerializeField]
    [Range(0, 1)]
    float damageTransfer;

    
    public override void DoEffect(CombatManager.Turn turn)
    {
        
    }
    public override StatusEffectInstance NewInstance(CombatManager.Turn owner)
    {
        return new GuardEffectInstance(this,owner);
    }

    public class GuardEffectInstance : DefensiveEffectInstance
    {
        public GuardEffectInstance(StatusEffect effect, CombatManager.Turn owner) : base(effect, owner)
        {
        }

        public override float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance)
        {

            float damageToTransfer = damage * (effect as GuardEffect).damageTransfer;
            effectInstance.owner.TakeDamage(damage, element, type);

            return damage - damageToTransfer;
        }
    }
}
