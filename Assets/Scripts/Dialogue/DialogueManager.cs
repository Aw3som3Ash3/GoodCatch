using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> dialogue;

    void Start()
    {
        dialogue = new Queue<string>();
    }

    public void StartDialogue (Dialogue dialogue)
    {
        
    }
}
