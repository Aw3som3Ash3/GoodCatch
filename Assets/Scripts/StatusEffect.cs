using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class StatusEffect : ScriptableObject 
{
    [SerializeField]
    int duration;
    

   
    public StatusEffectInstance NewInstance()
    {
        return new StatusEffectInstance(this);
    }
    protected abstract void DoEffect(CombatManager.Turn turn);

    public class StatusEffectInstance
    {
        int remainingDuration;
        StatusEffect effect;
        public StatusEffectInstance(StatusEffect effect)
        {
            remainingDuration = effect.duration;
            this.effect = effect;

        }
        public bool DoEffect(CombatManager.Turn turn)
        {
            remainingDuration--;
            effect.DoEffect(turn);
            return remainingDuration > 0;
        }
        public bool IsEffect(StatusEffect effect)
        {
            return this.effect == effect;
        }
        public void ResetEffect() 
        {
            remainingDuration = effect.duration;
        }
    }
}

