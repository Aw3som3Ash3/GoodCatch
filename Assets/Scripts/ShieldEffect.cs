using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : DefensiveEffect
{
    [SerializeField]
    float shieldHealth;
    public override void DoEffect(CombatManager.Turn turn)
    {
        //throw new System.NotImplementedException();
    }

    

    public class ShieldEffectInstance : DefensiveEffectInstance
    {
        float currentHealth;
        public ShieldEffectInstance(StatusEffect effect, FishMonster owner) : base(effect, owner)
        {
            currentHealth = (effect as ShieldEffect).shieldHealth;
        }

        public override float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance)
        {
            currentHealth -= damage;

            float remainingDamage = Mathf.Abs(currentHealth);
            if (currentHealth <= 0)
            {
                //remainingDuration = 0;
            }
            return remainingDamage;
            //throw new System.NotImplementedException();
        }
    }
}
