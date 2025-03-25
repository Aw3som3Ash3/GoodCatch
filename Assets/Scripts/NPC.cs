using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static NPC;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField]
    string npcName;
    public string StationName => $"Talk To {npcName}";

    public bool IsInteractable => !isTalking;
    bool isTalking;
    [SerializeField]
    Dialogue baseDialogue;
    Animator anim;
    [Serializable]
    public struct QuestBasedDialogue
    {
        [SerializeField]
        public Quest quest;
        [SerializeField]
        public string stateName;
        [SerializeField]
        public Dialogue dialogue;
    }

    [SerializeField]
    List<QuestBasedDialogue> questBasedDialogues;

    //[SerializeField]
    //List<UnityEvent> events;
    public virtual bool Interact()
    {

        isTalking = true;

        foreach (QuestBasedDialogue questBasedDialogue in questBasedDialogues)
        {
            if (QuestTracker.Instance.IsQuestStateActive(questBasedDialogue.quest, questBasedDialogue.stateName))
            {
                DialogueDisplayer.Instance.NewDialogue(questBasedDialogue.dialogue, anim, () => OnFinishedTalking());
                return true;
            }
        }
        DialogueDisplayer.Instance.NewDialogue(baseDialogue, anim, () => OnFinishedTalking());
        

        
        //throw new System.NotImplementedException();
        return true;
    }
    
    void OnFinishedTalking()
    {
        foreach (var quest in QuestTracker.Instance?.FindActiveRequirements<SpeakToNPCQuestRequirement>((x) => x.NpcName == npcName))
        {
            if (quest != null)
            {
                quest.RequirementCompleted();
            }
        }
        isTalking = false;
    }
    private void OnValidate()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        isTalking = false;
        anim = this.GetComponent<Animator>();
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
