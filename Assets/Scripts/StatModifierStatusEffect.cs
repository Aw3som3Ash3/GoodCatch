using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Modifier Effect", menuName = "Status Effect/Stat Modifier", order = 4)]
public class StatModifierStatusEffect : StatusEffect
{
    [SerializeField] int agility;
    //public int Agility { get { return agility; } }
    [SerializeField] int accuracy;
    //public int Accuracy { get { return accuracy; } }
    [SerializeField] int attack;
    //public int Attack { get { return attack; } }
    [SerializeField] int special;
    //public int Special { get { return special; } }
    [SerializeField] int fortitude;
    //public int Fortitude { get { return fortitude; } }
    [SerializeField] int specialFort;
    //public int SpecialFort { get { return specialFort; } }

    public Dictionary<string, int> Attribute { get; private set; } = new Dictionary<string, int>();
    public override void DoEffect(CombatManager.Turn turn)
    {
    }
    private void OnValidate()
    {
        Attribute["agility"] = agility;
        Attribute["accuracy"] = accuracy;
        Attribute["attack"] = attack;
        Attribute["special"] = special;
        Attribute["fortitude"] = fortitude;
        Attribute["specialFort"] = specialFort;
    }
    // Start is called before the first frame update

}
