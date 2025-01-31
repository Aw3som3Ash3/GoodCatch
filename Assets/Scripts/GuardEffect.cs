using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Guard", menuName = "Status Effect/Guard", order = 2)]
public class GuardEffect : DefensiveEffect
{
    [SerializeField]
    [Range(0, 1)]
    float damageTransfer;

    
    public override void DoEffect(CombatManager.Turn turn)
    {
        
    }
    public override StatusEffectInstance NewInstance(FishMonster owner)
    {
        return new GuardEffectInstance(this,owner);
    }

    public class GuardEffectInstance : DefensiveEffectInstance
    {
        public GuardEffectInstance(StatusEffect effect, FishMonster owner) : base(effect, owner)
        {
        }

        public override float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance)
        {

            float damageToTransfer = damage * (effect as GuardEffect).damageTransfer;
            Element.Effectiveness effectiveness;
            effectInstance.owner.TakeDamage(damage, element, type, out effectiveness);

            return damage - damageToTransfer;
        }
    }
}
