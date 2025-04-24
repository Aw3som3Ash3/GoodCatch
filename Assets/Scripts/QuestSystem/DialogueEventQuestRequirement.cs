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
        if (dialogueEvent != null)
        {
            
            dialogueEvent.Event += CompletedDialogueEvent;
        }
        else
        {
            Debug.LogWarning("Missing Quest Requirment Event");
        }
        base.Init();
    }
    public void CompletedDialogueEvent()
    {
        dialogueEvent.Event -= CompletedDialogueEvent;
        RequirementCompleted();
    }
    public override void ReInit()
    {
        if (dialogueEvent != null)
        {
            
            dialogueEvent.Event -= CompletedDialogueEvent;
            dialogueEvent.Event += CompletedDialogueEvent;
        }
        
        
    }
    public override void Clear()
    {
        //dialogueEvent.Event-=RequirementCompleted;
    }

}
