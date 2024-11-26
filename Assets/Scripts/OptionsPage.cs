using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class OptionsPage : VisualElement
{
    Button settings, save, load, mainMenu;
    VisualElement settingsBox;
    SaveAndLoadScreen saveAndLoadScreen;

    public new class UxmlFactory : UxmlFactory<OptionsPage, OptionsPage.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public OptionsPage()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/SettingsMenu.uxml");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        settings = this.Q<Button>("SettingsButton");
        settings.clicked += OnSettings;
        save = this.Q<Button>("SaveButton");
        save.clicked += OnSave;
        load = this.Q<Button>("LoadButton");
        load.clicked += OnLoad;
        mainMenu = this.Q<Button>("MainMenu");
        mainMenu.clicked += OnMenu;
        settingsBox = this.Q("SettingsBox");
        
    }
    public void OpenOptions()
    {
        if (saveAndLoadScreen != null&& settingsBox==saveAndLoadScreen.parent)
        {
            settingsBox.Remove(saveAndLoadScreen);

        }
        
    }
    private void OnSettings()
    {
        throw new NotImplementedException();
    }

    private void OnSave()
    {
        if (saveAndLoadScreen == null)
        {
            saveAndLoadScreen = new SaveAndLoadScreen();
            
        }
        settingsBox.Add(saveAndLoadScreen);
        saveAndLoadScreen.DisplaySaves();
    }

    private void OnLoad()
    {
        SavingSystem.LoadGame();
        //throw new NotImplementedException();
    }

    private void OnMenu()
    {
        throw new NotImplementedException();
    }
}
