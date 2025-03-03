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
    [SerializeField]
    FishMonsterType mudskipper,pyrishForSKippingQuest;

    [Serializable]
    struct TutorialItems
    {
        [SerializeField]
        public Item item;
        [SerializeField]
        public int amount;
    }
    [SerializeField]
    List<TutorialItems> items;

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
        foreach(TutorialItems item in items)
        {
            GameManager.Instance.PlayerInventory.AddItem(item.item, item.amount);
        }
        GameManager.Instance.CapturedFish(pyrishForSKippingQuest);
        QuestTracker.Instance.ForceCompleteQuest(QuestTracker.Instance.currentQuest.Quest);
        FinishedTutorial_Event();
    }
    private void FinishedTutorial_Event()
    {


        GameManager.Instance.PlayerFishventory.RemoveFishOfType(mudskipper);
        if (postTutorialQuest != null)
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
        
        SceneManager.LoadSceneAsync("Main Scene").completed+=(opertaion)=> { FindAnyObjectByType<SceneLoader>().AllScenesLoaded += () => { Inn.RemoveInnFromDictionary(Inn.StarterInn.innId); GameManager.Instance.ResetLastInn(); }; };
    }

    
}
