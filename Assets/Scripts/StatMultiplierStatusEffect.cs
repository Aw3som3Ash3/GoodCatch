using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Multiplier Effect", menuName = "Status Effect/Stat Multiplier", order = 4)]
public class StatMultiplierStatusEffect : StatusEffect
{
    [Range(-1.5f,1.5f)][SerializeField] float agility;
    //public int Agility { get { return agility; } }
    [Range(-1.5f, 1.5f)][SerializeField] float accuracy;
    //public int Accuracy { get { return accuracy; } }
    [Range(-1.5f, 1.5f)][SerializeField] float attack;
    //public int Attack { get { return attack; } }
    [Range(-1.5f, 1.5f)][SerializeField] float special;
    //public int Special { get { return special; } }
    [Range(-1.5f, 1.5f)][SerializeField] float fortitude;
    //public int Fortitude { get { return fortitude; } }
    [Range(-1.5f, 1.5f)][SerializeField] float specialFort;
    [Range(-1.5f, 1.5f)][SerializeField] float dodge;
    //public int SpecialFort { get { return specialFort; } }

    public Dictionary<string, float> Attribute { get; private set; } = new Dictionary<string, float>();
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
        Attribute["dodge"] = dodge;
        
    }
    private void OnEnable()
    {
        Attribute["agility"] = agility;
        Attribute["accuracy"] = accuracy;
        Attribute["attack"] = attack;
        Attribute["special"] = special;
        Attribute["fortitude"] = fortitude;
        Attribute["specialFort"] = specialFort;
        Attribute["dodge"] = dodge;
    }
    // Start is called before the first frame update

}
