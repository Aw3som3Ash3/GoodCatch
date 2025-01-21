using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornEffect : StatusEffect
{
    [SerializeField]
    float damage;
    [SerializeField]
    Element element;
    [SerializeField]
    Ability.AbilityType type;
    public override void DoEffect(CombatManager.Turn turn)
    {
        //throw new System.NotImplementedException();
    }

    public void ReflectDamage(FishMonster targetFish)
    {
        targetFish.TakeDamage(damage,element,type);
    }

    // Start is called before the first frame update
   
}
