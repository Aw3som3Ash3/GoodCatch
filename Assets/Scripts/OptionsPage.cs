using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OptionsPage : PausePage
{
    Button settings, controls, saveAndQuit, mainMenu,help;
    VisualElement settingsBox,menuContainer;
    SaveAndLoadScreen saveAndLoadScreen;
    SettingsUI settingsUI;
    ControlTabs controlTabs;
    
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
        settingsUI.visible = (false);
        //save = this.Q<Button>("SaveButton");
        //save.clicked += OnSave;
        //load = this.Q<Button>("LoadButton");
        //load.clicked += OnLoad;
        saveAndQuit = this.Q<Button>("SaveAndQuit");
        saveAndQuit.clicked += SaveAndQuit;
        mainMenu = this.Q<Button>("MainMenu");
        mainMenu.clicked += OnMenu;
        settingsBox = this.Q("SettingsBox");
        controlTabs = this.Q<ControlTabs>();
        controlTabs.visible=(false);
        controls=this.Q<Button>("Controls");
        controls.clicked += OnControls;
        menuContainer=this.Q("menu-container");
        help = this.Q<Button>("Help");
        help.clicked += OnHelp;
        menuContainer.Remove(controlTabs);
        menuContainer.Remove(settingsUI);
        this.focusable = true;
        this.delegatesFocus = true;

    }

    void OnHelp()
    {
        VisualElement helpRoot = new();
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ModHelp");

        visualTreeAsset.CloneTree(helpRoot);
        menuContainer.Clear();
        menuContainer.Add(helpRoot);

    }
    private void SaveAndQuit()
    {
        SavingSystem.SaveGame(SavingSystem.SaveMode.ManualSave);
        Application.Quit();
    }

    public void CloseAll()
    {
        settingsUI.visible = (false);
        controlTabs.visible = (false);
    }
    public void OpenOptions()
    {
       CloseAll();
        
        
    }
    void OnControls()
    {
        settingsUI.visible = (false);
        controlTabs.visible = (true);
        menuContainer.Add(controlTabs);
        controlTabs.StretchToParentSize();
        
        if (menuContainer.Contains(settingsUI))
        {
            menuContainer.Remove(settingsUI);
            menuContainer.Clear();
        }
      
    }
    private void OnSettings()
    {
        settingsUI.visible = (true);
        controlTabs.visible = (false);
        if (menuContainer.Contains(controlTabs))
        {
            menuContainer.Remove(controlTabs);
            menuContainer.Clear();
        }
        menuContainer.Add(settingsUI);
        //throw new NotImplementedException();
    }


    private void OnMenu()
    {
        SavingSystem.SaveGame(SavingSystem.SaveMode.ManualSave);
        GameObject.Destroy(QuestTracker.Instance.gameObject);
        GameObject.Destroy(GameManager.Instance.gameObject);
       
        SceneManager.LoadScene("MainMenu");
    }
}

public class OptionsPageMenu : VisualElement
{
    Button settings, controls, load, mainMenu,help;
    VisualElement settingsBox, menuContainer;

    SettingsUI settingsUI;
    ControlTabs controlTabs;

    public new class UxmlFactory : UxmlFactory<OptionsPageMenu, OptionsPageMenu.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public OptionsPageMenu()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/SettingsMainMenu");

        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        settings = this.Q<Button>("SettingsButton");
        settings.clicked += OnSettings;
        settingsUI = this.Q<SettingsUI>();
        settingsUI.visible = (false);
        //save = this.Q<Button>("SaveButton");
        //save.clicked += OnSave;
        //load = this.Q<Button>("LoadButton");
        //load.clicked += OnLoad;
        settingsBox = this.Q("SettingsBox");
        controlTabs = this.Q<ControlTabs>();
        controlTabs.visible = (false);
        controls = this.Q<Button>("Controls");
        controls.clicked += OnControls;
        menuContainer = this.Q("menu-container");
        help = this.Q<Button>("Help");
        help.clicked += OnHelp;
        menuContainer.Remove(controlTabs);
        menuContainer.Remove(settingsUI);
        this.focusable = true;
        this.delegatesFocus = true;


    }

    public void CloseAll()
    {
        settingsUI.visible = (false);
        controlTabs.visible = (false);
    }
    public void OpenOptions()
    {
        CloseAll();


    }
    void OnControls()
    {
        settingsUI.visible = (false);
        controlTabs.visible = (true);
        menuContainer.Add(controlTabs);
        controlTabs.StretchToParentSize();

        if (menuContainer.Contains(settingsUI))
        {
            menuContainer.Remove(settingsUI);
            menuContainer.Clear();
        }

    }
    private void OnSettings()
    {
        settingsUI.visible = (true);
        controlTabs.visible = (false);
        if (menuContainer.Contains(controlTabs))
        {
            menuContainer.Remove(controlTabs);
            menuContainer.Clear();
        }
        menuContainer.Add(settingsUI);
        //throw new NotImplementedException();
    }
    void OnHelp()
    {
        VisualElement helpRoot = new();
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ModHelp");

        visualTreeAsset.CloneTree(helpRoot);
        menuContainer.Clear();
        menuContainer.Add(helpRoot);

    }

}
