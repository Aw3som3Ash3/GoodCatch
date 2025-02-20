using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinNumFight : Quest.QuestRequirement
{
    [SerializeField]
    int amount;
    int current;
    public override string Objective => $"Win fights {current}/{amount}";

    public override void Init()
    {
        base.Init();
        current = 0;
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
