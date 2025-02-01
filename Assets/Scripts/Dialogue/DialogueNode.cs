using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public abstract class DialogueNode
{
    [SerializeField]
    [Multiline]
    public string dialouge;

    //public DialogueNode parent;
    

    public delegate void Action<in T1, in T2>(T1 previousNode, T2 nextNode);
    public delegate void Action<in T1>(T1 enteredNode);

    public event Action<DialogueNode> OnEntered;
    public event Action< DialogueNode,DialogueNode> OnExit;
    public string guid;

    public Vector2 position;


    public virtual void Enter()
    {
       OnEntered?.Invoke(this);
    }


    public virtual void Exit()
    {


    }

   
}


[Serializable]
public class BasicDialogue : DialogueNode
{
    [SerializeReference]
    public DialogueNode nextNode;

   
}

[Serializable]
public class BranchingDialogue : DialogueNode
{
    [SerializeField]
    public List<Decision> decisions=new();
    
    [Serializable]
    public class Decision
    {
        [SerializeField]
        public string choice;
        [SerializeReference]
        public DialogueNode choiceNode;

    }

   
}

[Serializable]
public class GiveQuest : BasicDialogue
{
    [SerializeField]
    public Quest quest;

    public override void Enter()
    {
        base.Enter();
        QuestTracker.Instance?.AddQuest(quest);
    }


}