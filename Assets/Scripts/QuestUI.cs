using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestUI : MonoBehaviour
{
    private Label questText;
    private Label subQuestHeader;
    private Label subQuestImport;

    private VisualElement checkBox;

    GameObject UIElement;

    bool hasFished = false;

    private void Awake()
    {
        UIElement = this.gameObject;
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        questText = rootVisualElement.Q<Label>("MainQuestImport");
        subQuestHeader = rootVisualElement.Q<Label>("SubQuests");
        subQuestImport = rootVisualElement.Q<Label>("SubQuestImport");

        checkBox = rootVisualElement.Q<VisualElement>("CheckboxHolder");

        subQuestHeader.visible = false;
        subQuestImport.visible = false;

        
        
    }
    private void Start()
    {
        /*Toggle toggle = new Toggle();
        toggle.name = "checkbox";
        toggle.text = "Complete?";
        questText.Add(toggle);*/

        //Toggle toggle2 = new Toggle();
        //toggle2.name = "checkbox2";
        //toggle2.text = "";
        //checkBox.Add(toggle2);

        QuestTracker.Instance.OnCurrentQuestUpdate += (q) => 
        { 
            Debug.Log("progressed on "+ q);
            if (q.CurrentState != null)
            {
                Debug.Log("now on " + q.CurrentState.Objective);
                questText.text = q.CurrentState.Objective;
            }
            else
            {
                questText.text = "no current quest";
            }
          
           
        };
        if (QuestTracker.Instance.currentQuest != null)
        {
            questText.text = QuestTracker.Instance.currentQuest.CurrentState.Objective;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
