using UnityEngine;
using static Element;

[CreateAssetMenu(fileName = "DoT Status Effect", menuName = "Status Effect/DoT", order = 1)]
public class DoTStatusEffect : StatusEffect
{

    [SerializeField]
    float damage,damageScale;
    [SerializeField]
    protected Element element;
    

    public override void DoEffect(CombatManager.Turn turn)
    {
        
        
    }
    public override StatusEffectInstance NewInstance(CombatManager.Turn owner)
    {
        return new DoTStatusEffectInstance(this, owner);
    }


    public class DoTStatusEffectInstance : StatusEffectInstance
    {
        public DoTStatusEffectInstance(StatusEffect effect, CombatManager.Turn owner) : base(effect, owner)
        {
            

        }


        public override bool DoEffect(CombatManager.Turn turn)
        {

            if (((DoTStatusEffect)effect).damage > 0)
            {
                turn.TakeDamage(((DoTStatusEffect)effect).damage+ (owner.special*0.01f) * ((DoTStatusEffect)effect).damageScale, ((DoTStatusEffect)effect).element, Ability.AbilityType.special);
            }
            else if (((DoTStatusEffect)effect).damage < 0)
            {
                turn.Restore(-((DoTStatusEffect)effect).damage + (owner.special * 0.01f) * ((DoTStatusEffect)effect).damageScale);
            }

            return base.DoEffect(turn);
        }
    }

}
