using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Healing Item", menuName = "Items/Healing Item", order = 3)]
public class HealingItem : CombatItem
{

    [SerializeField]
    float healingAmount;
}
