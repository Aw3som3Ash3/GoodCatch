using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Thorns", menuName = "Status Effect/Thorns", order = 2)]
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

    public void ReflectDamage(CombatManager.Turn targetFish)
    {
        
        targetFish.TakeDamage(damage,element,type);
    }

    // Start is called before the first frame update
   
}
