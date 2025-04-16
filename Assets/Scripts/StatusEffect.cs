using System;
using UnityEngine;


public abstract class StatusEffect: ScriptableObject
{
    [SerializeField]
    [Min(1)]
    int duration;

    [SerializeField]
    Texture2D icon;
    public Texture2D Icon { get { return icon; } }
    [SerializeField]
    ParticleSystem particlePrefab;
    public ParticleSystem ParticlePrefab { get { return particlePrefab; } }
    public enum EffectUsage
    {
        preTurn,
        postTurn,
        noTick
    }
    [SerializeField]
    EffectUsage effectUsage;


    public virtual StatusEffectInstance NewInstance(CombatManager.Turn owner)
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
        /// <summary>
        /// owner is the fish that casted the ability that proced the effect
        /// </summary>
        public CombatManager.Turn owner { get; private set; }
        public StatusEffectInstance(StatusEffect effect, CombatManager.Turn owner)
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
        public void ResetEffect(CombatManager.Turn newOwner)
        {
            remainingDuration = effect.duration;
            owner = newOwner;
        }
        public void ResetEffect()
        {
            remainingDuration = effect.duration;
            DurationChanged?.Invoke(remainingDuration);
        }
    }
}

