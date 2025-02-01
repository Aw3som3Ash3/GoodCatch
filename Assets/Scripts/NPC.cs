using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField]
    string npcName;
    public string StationName => $"Talk To {npcName}";
    [SerializeField]
    Dialogue dialogue;

    public virtual bool Interact()
    {

        foreach(var quest in QuestTracker.Instance?.FindActiveRequirments<SpeakToNPCQuestRequirement>((x) => x.NpcName == npcName))
        {
            if (quest != null) 
            {
                quest.RequirementCompleted();
            }
            
           
        }
        DialogueDisplayer.Instance.NewDialogue(dialogue);
        //throw new System.NotImplementedException();
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
       // var reader = new DialogueReader(dialogue);
        //reader.OnChoiceRequired += NPC_OnChoiceRequired;
        //reader.Next();

    }

    private DialogueNode NPC_OnChoiceRequired(Func<int, DialogueNode> action)
    {
        return action?.Invoke(1);
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
