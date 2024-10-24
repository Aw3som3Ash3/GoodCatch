using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoT Status Effect", menuName = "Status Effect/DoT", order = 1)]
public class DoTStatusEffect : StatusEffect
{

    [SerializeField]
    float damage;

    protected override void DoEffect(FishMonster fish)
    {
        fish.TakeDamage(damage, element, Ability.AbilityType.special);
    }

   
}
