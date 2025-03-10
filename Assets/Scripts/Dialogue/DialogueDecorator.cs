using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class DialogueDecorator
{
    public Vector2 position;
    public string guid;


    public abstract void Enter();
    public abstract void Exit();
}


public class DialogueEventDecorator : DialogueDecorator
{
    //public Action @Event;
    public DialogueEvent dialogueEvent;
    public DialogueEventDecorator()
    {




    }
#if UNITY_EDITOR

    //public override DialogueNode SetTree(Dialogue tree)
    //{
    //    dialogueEvent = ScriptableObject.CreateInstance<DialogueEvent>();
    //    AssetDatabase.AddObjectToAsset(dialogueEvent, tree);
    //    return base.SetTree(tree);
    //}

    public DialogueDecorator SetEvent(DialogueEvent _event)
    {

        
        dialogueEvent = _event;
        return this;
    }
#endif
    public override void Exit()
    {
        dialogueEvent.Invoke();
        //Event?.Invoke();
       
    }

    public override void Enter()
    {
        //throw new NotImplementedException();
    }
}


[Serializable]
public class GiveQuestDecorator : DialogueDecorator
{
    [SerializeField]
    public Quest quest;



    public override void Enter()
    {
        
        QuestTracker.Instance?.AddQuest(quest);
    }

    public override void Exit()
    {
        
    }
}

[Serializable]
public class CompleteArbritaryQuestDecorator : DialogueDecorator
{
    public string questTag;
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        foreach (var item in QuestTracker.Instance.FindActiveRequirements<ArbritaryQuestRequirment>((x) => x.Tag == questTag))
        {
            item.RequirementCompleted();

        }
    }
}