using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Healing Item", menuName = "Items/Healing Item", order = 3)]
public class HealingItem : Potion
{

    [SerializeField]
    float healingAmount;
    public float HealingAmount {  get { return healingAmount; } }

    protected override void PotionEffect(PlayerTurn target)
    {
        target.Restore(healingAmount);
    }
}
