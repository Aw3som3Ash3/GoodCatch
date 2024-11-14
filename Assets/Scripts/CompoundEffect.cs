using UnityEngine;


[CreateAssetMenu(fileName = "Compound Effect", menuName = "Status Effect/Compound", order = 5)]
public class CompoundEffect : StatusEffect
{
    [SerializeField]
    StatusEffect[] effects;
    public StatusEffect[] Effects { get { return effects; } }
    public override void DoEffect(CombatManager.Turn turn)
    {
        foreach (var effect in effects)
        {
            effect.DoEffect(turn);
        }
        //throw new System.NotImplementedException();
    }
}
