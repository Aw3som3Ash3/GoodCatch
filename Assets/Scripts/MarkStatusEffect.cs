using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    public override StatusEffectInstance NewInstance(FishMonster owner)
    {
        return new MarkStatusEffectInstance(this,owner);
    }

    public class MarkStatusEffectInstance : DefensiveEffectInstance
    {
        public MarkStatusEffectInstance(StatusEffect effect, FishMonster owner) : base(effect, owner)
        {

        }

        public override float MitigateDamage(float damage, Element element, Ability.AbilityType type, StatusEffectInstance effectInstance)
        {
            return damage+ damage*(effect as MarkStatusEffect).damageBonus;
        }
    }
}
