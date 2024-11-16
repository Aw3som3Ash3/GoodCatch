using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InnDialogue : MonoBehaviour
{
    private Label dialogue;
    private Button option1;
    private Button option2;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        dialogue = rootVisualElement.Q<Label>("#ChangeableText");
        option1 = rootVisualElement.Q<Button>("#Yes");
        option1 = rootVisualElement.Q<Button>("#No");

        dialogue.text = "Welcome to the inn, would you like to stay?";
    }
}
