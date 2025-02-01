using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public class DialogueDisplayer : MonoBehaviour
{
    public static DialogueDisplayer Instance;
    DialogueReader reader;

    Label dialogueText;
    Button option1, option2;
    VisualElement rootVisualElement;
    public void NewDialogue(Dialogue dialogue)
    {
       
        rootVisualElement.visible = true;
        reader =new DialogueReader(dialogue);
        var inputs = new GoodCatchInputs();
        inputs.UI.NextDialogue.performed += (x) => reader.Next();
        inputs.UI.NextDialogue.Enable();
        reader.NextDialogue += (text) => 
        { 
            dialogueText.text = text; 
            option1.visible = false; 
            option2.visible = false; 
           
            
        };
        reader.OnCompleted += () =>
        {
            rootVisualElement.visible = false;
            reader.Dispose();
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            InputManager.EnablePlayer();
            inputs.UI.NextDialogue.Disable();
            inputs.Dispose();
        };
        //option1.clicked += () =>;
        
        option1.clicked += () => reader.Choose(0);
        option2.clicked += () => reader.Choose(1);
       
        reader.ChoiceRequired += (decisions) => 
        {
            option1.text = decisions[0].choice;
            option2.text = decisions[1].choice;
            option1.visible = true; 
            option2.visible = true;
        };
        reader.Start();

        

        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        InputManager.DisablePlayer();

        
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //UIElement = this.gameObject;
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        dialogueText = rootVisualElement.Q<Label>("InnSpeak");
        option1 = rootVisualElement.Q<Button>("Yes");
        option2 = rootVisualElement.Q<Button>("No");
        rootVisualElement.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
