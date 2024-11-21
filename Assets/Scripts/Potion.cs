using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Potion : CombatItem
{
    [SerializeField]
    protected ParticleSystem potionVFX;

    public void UsePotion(PlayerTurn turn,Action<ParticleSystem> potionVFX)
    {
        if (this.potionVFX != null)
        {
            potionVFX?.Invoke(this.potionVFX);
        }
        PotionEffect(turn);
    }
    protected abstract void PotionEffect(PlayerTurn turn);
    
}
