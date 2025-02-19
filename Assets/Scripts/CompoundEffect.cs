using System.Collections.Generic;
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
    public override StatusEffectInstance NewInstance(CombatManager.Turn owner)
    {
        return new CompoundEffectInstance(this,owner);
    }
    public class CompoundEffectInstance : StatusEffectInstance
    {
       List<StatusEffectInstance> effectInstances=new();
        public CompoundEffectInstance(CompoundEffect effect, CombatManager.Turn owner) : base(effect, owner)
        {
            foreach (var _effect in effect.effects)
            {
                effectInstances.Add(_effect.NewInstance(owner));
            }
            

        }

        
    }
}
