using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakToNPCQuestRequirement : Quest.QuestRequirement
{
    [SerializeField]
    readonly public string npcName;
    public override string Objective => $"Speak to {npcName}";

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

   
}
