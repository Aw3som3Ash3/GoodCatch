using UnityEngine;

[CreateAssetMenu(fileName = "Stun", menuName = "Status Effect/Stun", order = 2)]
public class StunStatusEffect : StatusEffect
{
    public override void DoEffect(CombatManager.Turn turn)
    {

        turn.UseAction(10);
        //turn.EndTurn();
    }

}
