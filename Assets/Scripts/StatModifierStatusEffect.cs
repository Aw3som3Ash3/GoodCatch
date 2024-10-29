using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Modifier Effect", menuName = "Status Effect/Stat Modifier", order = 4)]
public class StatModifierStatusEffect : StatusEffect
{
    [SerializeField] int agility;
    public int Agility { get { return agility; } }
    [SerializeField] int accuracy;
    public int Accuracy { get { return accuracy; } }
    [SerializeField] int attack;
    public int Attack { get { return attack; } }
    [SerializeField] int special;
    public int Special { get { return special; } }
    [SerializeField] int fortitude;
    public int Fortitude { get { return fortitude; } }
    [SerializeField] int specialFort;
    public int SpecialFort { get { return specialFort; } }
    protected override void DoEffect(CombatManager.Turn turn)
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
}
