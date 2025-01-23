using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MudskipperDialogue : MonoBehaviour
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
    }

    private void Start()
    {
        DisableUI();
    }

    private void DisableUI()
    {
        UIElement.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Time.timeScale = 1.0f;
    }
    private void ShowUI()
    {
        Debug.Log("ShowUI function has been called.");
        UIElement.SetActive(true);
        option1.visible = false;
        option2.visible = false;
    }

    private void ShowUISelectable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 0;
        InputManager.DisablePlayer();
        Debug.Log("ShowUISelectable function has been called.");
        UIElement.SetActive(true);
        option1.visible = true;
        option1.Focus();
        option2.visible = true;
    }

    public void DisplayFirstOption(Action onClick)
    {
        ShowUISelectable();
        dialogue.text = "There is no tutorial yet... Wake Up!";
        //option1.clicked += () => { onClick?.Invoke(); DisableUI(); };
        //option2.clicked += DisableUI;
    }
}