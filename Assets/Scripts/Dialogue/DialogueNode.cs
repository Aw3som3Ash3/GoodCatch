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
    [SerializeField]
    public AudioClip voiceClip;



    //public DialogueNode parent;
    protected Dialogue tree;

    public delegate void Action<in T1, in T2>(T1 previousNode, T2 nextNode);
    public delegate void Action<in T1>(T1 enteredNode);

    public event Action<DialogueNode> OnEntered;
    public event Action< DialogueNode,DialogueNode> OnExit;
    public string guid;
    [SerializeField]
    [SerializeReference]
    public List<DialogueDecorator> decorators=new();
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
        foreach(var decorator in decorators)
        {
            decorator.Enter();
        }
        OnEntered?.Invoke(this);
    }


    public virtual void Exit()
    {
        foreach (var decorator in decorators)
        {
            decorator.Exit();
        }

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




