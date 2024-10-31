using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class StatusEffect : ScriptableObject 
{
    [SerializeField]
    int duration;
   
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get { return icon; } }

    

   
    public StatusEffectInstance NewInstance()
    {
        return new StatusEffectInstance(this);
    }
    public abstract void DoEffect(CombatManager.Turn turn);

    public class StatusEffectInstance
    {
        public int remainingDuration { get; private set; }
        public StatusEffect effect { get; private set; }
        public Action<int> DurationChanged;
        public StatusEffectInstance(StatusEffect effect)
        {
            remainingDuration = effect.duration;
            this.effect = effect;

        }
        public bool DoEffect(CombatManager.Turn turn)
        {
            remainingDuration--;
            DurationChanged?.Invoke(remainingDuration);
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

