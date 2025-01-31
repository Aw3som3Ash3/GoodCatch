using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField]
    string npcName;
    public string StationName => $"Talk To {npcName}";

    public virtual bool Interact()
    {

        foreach(var quest in QuestTracker.Instance?.FindActiveRequirments<SpeakToNPCQuestRequirement>((x) => x.NpcName == npcName))
        {
            if (quest != null) 
            {
                quest.RequirementCompleted();
            }
            
           
        }
        //throw new System.NotImplementedException();
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
