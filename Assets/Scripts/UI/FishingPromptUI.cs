using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishingPromptUI : MonoBehaviour
{
    GameObject UIElement;


    private void OnEnable()
    {
        UIElement = this.gameObject;
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
    }

    void Start()
    {
        UIElement.SetActive(false);
    }

    public void ShowUI()
    {
        UIElement.SetActive(true);
    }

    public void DisableUI()
    {
        UIElement.SetActive(false);
    }
}
