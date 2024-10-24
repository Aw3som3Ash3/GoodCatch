using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class StatusEffect : ScriptableObject
{
    [SerializeField]
    int duration;
    [SerializeField]
    protected Element element;

   
    public StatusEffectInstance NewInstance()
    {
        return new StatusEffectInstance(this);
    }
    protected abstract void DoEffect(FishMonster fish);

    public class StatusEffectInstance
    {
        int remainingDuration;
        StatusEffect effect;
        public StatusEffectInstance(StatusEffect effect)
        {
            remainingDuration = effect.duration;
            this.effect = effect;

        }
        public void DoEffect(FishMonster fish)
        {
            effect.DoEffect(fish);
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

