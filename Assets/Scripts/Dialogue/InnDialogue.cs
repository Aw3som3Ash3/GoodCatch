using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InnDialogue : MonoBehaviour
{
    private Label dialogue;
    private Button option1;
    private Button option2;

    GameObject UIElement;
    


    private void OnEnable()
    {
        UIElement = this.gameObject;
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        dialogue = rootVisualElement.Q<Label>("InnSpeak");
        option1 = rootVisualElement.Q<Button>("Yes");
        option2 = rootVisualElement.Q<Button>("No");

        //dialogue.text = "Welcome to the inn, would you like to stay?";

        //option2.RegisterCallback<ClickEvent>(ev => DisableUI());
    }

    private void Start()
    {
        DisableUI();
    }

    private void DisableUI()
    {
        UIElement.SetActive(false);
    }
    private void ShowUI()
    {
        Debug.Log("ShowUI function has been called.");
        UIElement.SetActive(true);
    }

    public void DisplayFirstOption()
    {
        ShowUI();
        dialogue.text = "Welcome to the inn, would you like to stay?";

        //option1.RegisterCallback<ClickEvent>(ev => DisableUI());
    }

    public void CantSleepMessage()
    {
        ShowUI();
        StartCoroutine(CantSleepText());
    }

    private IEnumerator CantSleepText()
    {
        dialogue.text = "It is not night time... You can't sleep.";
        yield return new WaitForSeconds(2.5f);
        DisableUI();
    }
}
