using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun", menuName = "Status Effect/Stun", order = 2)]
public class StunStatusEffect : StatusEffect
{
    protected override void DoEffect(CombatManager.Turn turn)
    {
       
        turn.UseAction(-10);
        //turn.EndTurn();
    }

}
