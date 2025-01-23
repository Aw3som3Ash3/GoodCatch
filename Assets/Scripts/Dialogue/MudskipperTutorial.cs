using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MudskipperTutorial : MonoBehaviour, IInteractable
{
    public string StationName => "Mudskipper";
    public MudskipperDialogue dialogue;

    void Awake()
    {
        dialogue = FindObjectOfType<MudskipperDialogue>(true);
    }

    public bool Interact()
    {
        dialogue.DisplayFirstOption(LoadMainScene);
        return true;
    }    

    void LoadMainScene()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
