using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventQuestRequirement : Quest.QuestRequirement
{
    [SerializeField]
    string objective;
    [SerializeField]
    DialogueEvent dialogueEvent;
    public override string Objective => objective;

    public override void Init()
    {
        dialogueEvent.Event += RequirementCompleted;
        
    }

    
}
