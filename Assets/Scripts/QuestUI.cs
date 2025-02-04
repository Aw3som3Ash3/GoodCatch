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

    GameObject UIElement;

    bool hasFished = false;

    private void Awake()
    {
        UIElement = this.gameObject;
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        questText = rootVisualElement.Q<Label>("MainQuestImport");
        subQuestHeader = rootVisualElement.Q<Label>("SubQuests");
        subQuestImport = rootVisualElement.Q<Label>("SubQuestImport");

        subQuestHeader.visible = false;
        subQuestImport.visible = false;

        
        
    }
    private void Start()
    {
        QuestTracker.Instance.OnQuestUpdate += (q) => 
        { 
            Debug.Log("progressed on "+ q); 
            Debug.Log("now on "+ q.CurrentState.Objective); 
            questText.text = q.CurrentState.Objective; 
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
