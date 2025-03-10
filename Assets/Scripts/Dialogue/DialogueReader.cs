using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BranchingDialogue;

public class DialogueReader : IDisposable
{
    Dialogue dialogue;
    DialogueNode currentNode;
    private bool disposedValue;

    //public event Func<Func<int, DialogueNode>,DialogueNode> OnChoiceRequired;
    /// <summary>
    /// an event that outputs the next dialogue string
    /// </summary>
    public event Action<DialogueNode> NextDialogue;
    /// <summary>
    /// event that is triggered when the reader reaches a branch node
    /// </summary>
    public event Action< List<BranchingDialogue.Decision>> ChoiceRequired;
    /// <summary>
    /// event that is triggered when the dialogue is completed
    /// </summary>
    public event Action OnCompleted;
    public DialogueReader(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        //currentNode = dialogue.rootNode.nextNode;

    }
    /// <summary>
    /// Starts the reading of the dialogue tree
    /// </summary>
    public void Start()
    {
        currentNode = dialogue.rootNode.nextNode;
        OnEnter();
    }
    void OnEnter()
    {
        if (currentNode == null)
        {
            Debug.Log("completed dialogue");
            OnCompleted?.Invoke();

            return;
        }
        currentNode.Enter();
        NextDialogue.Invoke(currentNode);
        if (currentNode is BranchingDialogue)
        {
            var branchingDialogue = currentNode as BranchingDialogue;
            ChoiceRequired?.Invoke(branchingDialogue.decisions);
            
        }
    }
    public void Next()
    {
        if(!(currentNode is BranchingDialogue))
        {
            currentNode.Exit();
            currentNode = (currentNode as BasicDialogue).nextNode;
            OnEnter();
        }
        
       
    }
    public void Choose(int index)
    {
        if (currentNode is BranchingDialogue)
        {
            Debug.Log(index);
            currentNode.Exit();
            currentNode = (currentNode as BranchingDialogue).decisions[index].choiceNode;
            OnEnter();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            dialogue = null;
            currentNode = null;
            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DialogueReader()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }


    //public DialogueNode Choose(int index) 
    //{
    //    //var branchNode = currentNode as BranchingDialogue;
    //    //return branchNode.decisions[index].choiceNode;

    //}


}
