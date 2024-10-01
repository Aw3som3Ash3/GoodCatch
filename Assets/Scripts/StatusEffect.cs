using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StatusEffect : ScriptableObject
{
    [SerializeField]
    int duration;
    [SerializeField]
    Element element;

    public StatusEffectInstance NewInstance()
    {
        return new StatusEffectInstance(this);
    }

    public class StatusEffectInstance
    {
        int remainingDuration;

        public StatusEffectInstance(StatusEffect effect)
        {
            remainingDuration = effect.duration;
        }
    }
}

