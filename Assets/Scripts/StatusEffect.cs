using System;
using UnityEngine;


public abstract class StatusEffect : ScriptableObject
{
    [SerializeField]
    int duration;

    [SerializeField]
    Texture2D icon;
    public Texture2D Icon { get { return icon; } }
    public enum EffectUsage
    {
        preTurn,
        postTurn
    }
    [SerializeField]
    EffectUsage effectUsage;


    public StatusEffectInstance NewInstance(FishMonster owner)
    {
        return new StatusEffectInstance(this,owner);
    }
    public abstract void DoEffect(CombatManager.Turn turn);

    public class StatusEffectInstance
    {
        public int remainingDuration { get; private set; }
        public StatusEffect effect { get; private set; }
        public Action<int> DurationChanged;
        public EffectUsage effectUsage { get { return effect.effectUsage; } }
        public FishMonster owner { get; private set; }
        public StatusEffectInstance(StatusEffect effect,FishMonster owner)
        {
            remainingDuration = effect.duration;
            this.effect = effect;
            this.owner = owner;
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
        public void ResetEffect(FishMonster newOwner)
        {
            remainingDuration = effect.duration;
            owner = newOwner;
        }
        public void ResetEffect()
        {
            remainingDuration = effect.duration;
            
        }
    }
}

