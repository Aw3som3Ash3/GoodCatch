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

    private void OnEnable()
    {
        UIElement = this.gameObject;
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        questText = rootVisualElement.Q<Label>("MainQuestImport");
        subQuestHeader = rootVisualElement.Q<Label>("SubQuests");
        subQuestImport = rootVisualElement.Q<Label>("SubQuestImport");

        subQuestHeader.visible = false;
        subQuestImport.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasFished == false)
        {
            questText.text = "Go Fishing";
        }
    }
}
