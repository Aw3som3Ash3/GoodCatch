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
    DialogueEvent healFish;
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
            skipTutorial.Event += SkippedTutorial;
        }
        if (healFish != null)
        {
            healFish.Event += () => { GameManager.Instance.RestoreFish(); };
        }
       
        //dialogue = FindObjectOfType<MudskipperDialogue>(true);
    }


    void SkippedTutorial()
    {
        //GameManager.Instance.

        FinishedTutorial_Event();
    }
    private void FinishedTutorial_Event()
    {
       


        if (postTutorialQuest != null)
        {
            QuestTracker.Instance.ForceCompleteQuest(QuestTracker.Instance.currentQuest.Quest);

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
