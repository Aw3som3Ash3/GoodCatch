using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Status Potion", menuName = "Items/Status Potion", order = 3)]
public class StatusPotion : Potion
{
    [SerializeField]
    StatusEffect statusEffect;
    public StatusEffect StatusEffect { get => statusEffect; }
    protected override void PotionEffect(PlayerTurn turn)
    {
        turn.AddEffects(statusEffect,turn);
    }

   
}
