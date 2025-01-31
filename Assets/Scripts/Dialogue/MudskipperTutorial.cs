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
    public MudskipperDialogue dialogue;

    void Awake()
    {
        dialogue = FindObjectOfType<MudskipperDialogue>(true);
    }

    public override bool Interact()
    {

        dialogue.DisplayFirstOption(LoadMainScene);
        return base.Interact();
    }    

    void LoadMainScene()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
