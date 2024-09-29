using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoT Status Effect", menuName = "Status Effect/DoT", order = 1)]
public class DoTStatusEffect : StatusEffect
{

    [SerializeField]
    float damage;

    public class DoTInstance:StatusEffectInstance
    {
        int remainingDuration;
        float damage;

        public DoTInstance(StatusEffect effect) : base(effect)
        {
            damage = ((DoTStatusEffect)effect).damage;
        }

        public bool TickDamage(out float damage)
        {
            remainingDuration--;

            if (remainingDuration > 0)
            {

                damage = this.damage;
                return true;
            }
            else
            {
                damage = 0f;
                return false;
            }

        }

    }
}
