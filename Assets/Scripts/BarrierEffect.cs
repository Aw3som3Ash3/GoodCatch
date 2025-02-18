using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Barrier", menuName = "Status Effect/Defensive Effects/Barrier", order = 2)]
public class BarrierEffect : DefensiveEffect
{

    public override void DoEffect(CombatManager.Turn turn)
    {
        //throw new System.NotImplementedException();
    }


    public override StatusEffectInstance NewInstance(CombatManager.Turn owner)
    {
        return new BarrierEffectInstance(this, owner);
    }
    public class BarrierEffectInstance : DefensiveEffectInstance
    {
        public BarrierEffectInstance(StatusEffect effect, CombatManager.Turn owner) : base(effect, owner)
        {

        }

        public override float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance)
        {
            DoEffect(null);
            return 0;
        }
    }
}
