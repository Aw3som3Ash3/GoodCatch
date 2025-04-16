using UnityEngine;
using static Element;

[CreateAssetMenu(fileName = "DoT Status Effect", menuName = "Status Effect/DoT", order = 1)]
public class DoTStatusEffect : StatusEffect
{

    [SerializeField]
    float damage;
    [SerializeField]
    protected Element element;

    public override void DoEffect(CombatManager.Turn turn)
    {
        if (damage > 0)
        {
            turn.TakeDamage(damage, element, Ability.AbilityType.special);
        }else if (damage < 0)
        {
            turn.Restore(-damage);
        }
        
    }


}
