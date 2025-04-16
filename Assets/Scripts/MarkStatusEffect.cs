using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Mark", menuName = "Status Effect/Mark", order = 2)]
public class MarkStatusEffect : DefensiveEffect
{

    [SerializeField]
    [Range(0, 2)]
    float damageBonus;
    public MarkStatusEffect()
    {
    }

    public override void DoEffect(CombatManager.Turn turn)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override StatusEffectInstance NewInstance(CombatManager.Turn owner)
    {
        return new MarkStatusEffectInstance(this,owner);
    }

    public class MarkStatusEffectInstance : DefensiveEffectInstance
    {
        public MarkStatusEffectInstance(StatusEffect effect, CombatManager.Turn owner) : base(effect, owner)
        {

        }

        public override float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance)
        {
            DoEffect(owner);
            
            return damage+ damage*(effect as MarkStatusEffect).damageBonus;
        }
    }
}
