using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    UIDocument uIDocument;
    VisualElement mainMenu,mainScreen,loadScreen;
    private void Awake()
    {
        uIDocument = GetComponent<UIDocument>();
        mainMenu = uIDocument.rootVisualElement.Q("MainMenu");
        mainScreen = uIDocument.rootVisualElement.Q("MainScreen");
        loadScreen = uIDocument.rootVisualElement.Q("LoadGameScreen");
    }
    // Start is called before the first frame update
    void Start()
    {
        mainScreen.Q<Button>("NewGame").Focus();
        mainScreen.Q<Button>("LoadGame").clicked += () =>
        {
            mainScreen.visible=false;
            loadScreen.visible=true;
            LoadGame();
        };
        mainScreen.Q<Button>("NewGame").clicked += () =>
        {
            mainScreen.visible = false;
            loadScreen.visible = true;
            NewGame();
        };
        mainScreen.Q<Button>("Quit").clicked += () =>
        {
            Application.Quit();
        };

    }
    void NewGame()
    {
        loadScreen.Q<Label>("LoadTitle").text = "New Game";
        for (int i = 1; i <= 3; i++)
        {
            int index = i;

            var button = loadScreen.Q<Button>("Slot" + i);
            button.SetEnabled(true);
            button.clicked += () => 
            { 
                SavingSystem.SetSlot(index); 
                SavingSystem.ClearSlot(index);
                //SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadSceneAsync("IntroScene").completed+=OnSceneLoaded;
                
                


            };
            
            
        }
    }

    private void OnSceneLoaded(AsyncOperation operation)
    {
        PlayableDirector playableDirector;
        playableDirector = FindObjectOfType<PlayableDirector>();
        var mainSceneLoading = SceneManager.LoadSceneAsync("Main Scene");
        mainSceneLoading.allowSceneActivation = false;

        playableDirector.stopped += (x) =>
        {
            mainSceneLoading.allowSceneActivation = true;
            mainSceneLoading.completed += (x) =>
            {
                FindAnyObjectByType<SceneLoader>().AllScenesLoaded += () => SavingSystem.SaveGame(SavingSystem.SaveMode.AutoSave);

            };

        };
       
       
    }

    //void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    //{
        
       
        
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    void LoadGame()
    {
        for (int i = 1; i <= 3; i++)
        {
            int index = i;

            var button = loadScreen.Q<Button>("Slot" + i);
            if (SavingSystem.HasSlot(i))
            {
                button.clicked += () => { SavingSystem.LoadGame(index); };
                button.SetEnabled(true);
            }
            else
            {
                button.SetEnabled(false);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
