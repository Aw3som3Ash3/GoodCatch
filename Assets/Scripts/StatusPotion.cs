using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPotion : Potion
{
    [SerializeField]
    StatusEffect statusEffect;
    protected override void PotionEffect(PlayerTurn turn)
    {
        turn.AddEffects(statusEffect,turn.fish);
    }

   
}
