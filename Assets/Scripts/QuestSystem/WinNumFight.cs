using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinNumFight : Quest.QuestRequirement
{
    [SerializeField]
    int amount;
    int current;
    public override string Objective => $"Win {amount} of fights";

    public override void Init()
    {
        GameManager.Instance.WonFight += () =>
        {
            current++;
            if (current >= amount)
            {
                RequirementCompleted();
            }
            else
            {
                RequirementProgressed();
            }
        };
    }
    
}
