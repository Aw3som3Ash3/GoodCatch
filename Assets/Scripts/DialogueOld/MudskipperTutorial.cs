using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MudskipperTutorial : NPC
{

    //public string StationName => "Mudskipper";
    //public MudskipperDialogue dialogue;
    [SerializeField]
    DialogueEvent skipTutorial;
    [SerializeField]
    DialogueEvent finishedTutorial;
    [SerializeField]
    Quest postTutorialQuest;

    void Awake()
    {
        if (finishedTutorial != null)
        {
            finishedTutorial.Event += FinishedTutorial_Event;
        }
        if (skipTutorial != null)
        {
            skipTutorial.Event += FinishedTutorial_Event;
        }
       
        //dialogue = FindObjectOfType<MudskipperDialogue>(true);
    }

    private void FinishedTutorial_Event()
    {
       
        if (postTutorialQuest == null)
        {
            QuestTracker.Instance.AddQuest(postTutorialQuest, true);
        }
        LoadMainScene();
    }

    public override bool Interact()
    {
       
        //dialogue.DisplayFirstOption(LoadMainScene);
        return base.Interact();
    }    

    void LoadMainScene()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
