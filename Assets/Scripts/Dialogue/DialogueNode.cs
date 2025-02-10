using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

[Serializable]
public abstract class DialogueNode
{
    [SerializeField]
    [Multiline]
    public string dialouge;

    //public DialogueNode parent;
    protected Dialogue tree;

    public delegate void Action<in T1, in T2>(T1 previousNode, T2 nextNode);
    public delegate void Action<in T1>(T1 enteredNode);

    public event Action<DialogueNode> OnEntered;
    public event Action< DialogueNode,DialogueNode> OnExit;
    public string guid;

    public Vector2 position;
#if UNITY_EDITOR
    public virtual DialogueNode SetTree(Dialogue tree)
    {
        this.tree = tree;
        return this;
    }
#endif
    public DialogueNode()
    {
       
    }
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

public class DialogueEventNode : BasicDialogue
{
    //public Action @Event;
    public DialogueEvent dialogueEvent;
    public DialogueEventNode()
    {
       
        
       

    }
#if UNITY_EDITOR
    public override DialogueNode SetTree(Dialogue tree)
    {
        dialogueEvent = ScriptableObject.CreateInstance<DialogueEvent>();
        AssetDatabase.AddObjectToAsset(dialogueEvent, tree);
        return base.SetTree(tree);
    }

    public  DialogueNode SetEvent(Dialogue tree,DialogueEvent _event)
    {

        AssetDatabase.RemoveObjectFromAsset(dialogueEvent);
        ScriptableObject.Destroy(dialogueEvent);
        dialogueEvent =_event;
        return base.SetTree(tree);
    }
#endif
    public override void Exit()
    {
        dialogueEvent.Invoke();
        //Event?.Invoke();
        base.Exit();
    }
}