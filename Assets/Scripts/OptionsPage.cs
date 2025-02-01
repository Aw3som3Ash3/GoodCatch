using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OptionsPage : PausePage
{
    Button settings, save, load, mainMenu;
    VisualElement settingsBox;
    SaveAndLoadScreen saveAndLoadScreen;
    SettingsUI settingsUI;

    public new class UxmlFactory : UxmlFactory<OptionsPage, OptionsPage.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public OptionsPage()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/SettingsMenu");

        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        settings = this.Q<Button>("SettingsButton");
        settings.clicked += OnSettings;
        settingsUI = this.Q<SettingsUI>();
        settingsUI.visible = false;
        //save = this.Q<Button>("SaveButton");
        //save.clicked += OnSave;
        //load = this.Q<Button>("LoadButton");
        //load.clicked += OnLoad;
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
        settingsUI.visible = true;
        //throw new NotImplementedException();
    }

    private void OnSave()
    {
        SavingSystem.SaveGame(SavingSystem.SaveMode.ManualSave);
        //if (saveAndLoadScreen == null)
        //{
        //    saveAndLoadScreen = new SaveAndLoadScreen();
            
        //}
        //settingsBox.Add(saveAndLoadScreen);
        //saveAndLoadScreen.DisplaySaves(SaveAndLoadScreen.Mode.save);
        //saveAndLoadScreen.Q<TextField>("SaveField").Focus();
    }

    private void OnLoad()
    {
        if (saveAndLoadScreen == null)
        {
            saveAndLoadScreen = new SaveAndLoadScreen();

        }
        settingsBox.Add(saveAndLoadScreen);
        saveAndLoadScreen.DisplaySaves(SaveAndLoadScreen.Mode.load);
    }

    private void OnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
