using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    UIDocument uIDocument;
    VisualElement mainMenu, mainScreen, loadScreen;
    OptionsPageMenu optionsScreen;
    [SerializeField]
    public AudioMixer mixer;

    [SerializeField]
    GameObject[] startingObjects;

    Action<int> startGameEvent;
    private void Awake()
    {
        //InputManager.Init();
        uIDocument = GetComponent<UIDocument>();
        mainMenu = uIDocument.rootVisualElement.Q("MainMenu");
        mainScreen = uIDocument.rootVisualElement.Q("MainScreen");
        loadScreen = uIDocument.rootVisualElement.Q("LoadGameScreen");
        optionsScreen = uIDocument.rootVisualElement.Q<OptionsPageMenu>();
        optionsScreen.visible = false;
        InputManager.Input.UI.Back.performed+=Back;
        InputManager.Input.UI.Back.Enable();
        GameManager.Instance = null;
        QuestTracker.Instance = null;
        InputManager.OnInputChange += OnInputChange;



        for (int i = 1; i <= 3; i++)
        {
            int index = i;
            var button = loadScreen.Q<Button>("Slot" + i);

            //button.SetEnabled(true);
            button.clicked += () =>
            {
                startGameEvent.Invoke(index);
                for (int j = 0; j < startingObjects.Length; j++)
                {
                    //Instantiate(startingObjects[j]);
                }
            };

            Time.timeScale = 1;
        }

    }

    void OnInputChange(InputMethod inputMethod)
    {
        if (inputMethod == InputMethod.mouseAndKeyboard)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }
    private void OnDestroy()
    {
        InputManager.OnInputChange -= OnInputChange;

    }
    private void Back(InputAction.CallbackContext context)
    {
        if (mainScreen.visible != true) 
        {
            loadScreen.visible = false;
            mainScreen.visible = true;
            optionsScreen.visible = false;
            optionsScreen.CloseAll();
        }
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

        mainScreen.Q<Button>("Options").clicked += () =>
        {
            optionsScreen.visible = true;
            mainScreen.visible = false;
        };

    }
    void NewGame()
    {
        loadScreen.Q<Label>("LoadTitle").text = "New Game";
        for (int i = 1; i <= 3; i++)
        {
            int index = i;
            var button = loadScreen.Q<Button>("Slot" + i);
            if (!SavingSystem.HasSlot(i))
            {
                button.text = "Empty " + (i);

            }
            else
            {
                button.text = "Slot " + (i);
            }
            button.SetEnabled(true);

            
        }
        Time.timeScale = 1;
        startGameEvent = (index) =>
        {
            SavingSystem.SetSlot(index);
            SavingSystem.ClearSlot(index);
            //SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync("IntroScene").completed += OnSceneLoaded;
            InputManager.Input.UI.Back.performed -= Back;
        };
       

    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Main Scene")
        {
            FindAnyObjectByType<SceneLoader>().AllScenesLoaded += () => { SavingSystem.SaveGame(SavingSystem.SaveMode.AutoSave);  GameManager.Instance.ResetLastInn(); } ;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
        }
    }
    private void OnSceneLoaded(AsyncOperation operation)
    {
        PlayableDirector playableDirector;
        playableDirector = FindObjectOfType<PlayableDirector>();
        var mainSceneLoading = SceneManager.LoadSceneAsync("DreamIsland");
        mainSceneLoading.allowSceneActivation = false;
        InputAction action = new();
        GoodCatchInputs uIActions = new GoodCatchInputs();
        uIActions.UI.SkipCutscene.Enable();
        uIActions.UI.SkipCutscene.performed += (x) => playableDirector.Stop();
        //InputSystem.onAnyButtonPress.CallOnce((x) => playableDirector.Stop() );
        playableDirector.stopped += (x) =>
        {
            uIActions.Disable();
            //Destroy(uIActions);
            mainSceneLoading.allowSceneActivation = true;
            mainSceneLoading.completed += (x) =>
            {
               

            };

        };
       
       
    }
    void LoadGame()
    {
        loadScreen.Q<Label>("LoadTitle").text = "Load Game";
        for (int i = 1; i <= 3; i++)
        {
            int index = i;

            var button = loadScreen.Q<Button>("Slot" + i);
            if (!SavingSystem.HasSlot(i))
            {
                button.text = "Empty " + (i);

            }
            else
            {
                button.text = "Slot " + (i);
            }
            if (SavingSystem.HasSlot(i))
            {
                button.SetEnabled(true);
            }
            else
            {
                button.SetEnabled(false);
            }

        }
        Time.timeScale = 1;

        startGameEvent = (index) =>
        {
            SavingSystem.LoadGame(index); 
            InputManager.Input.UI.Back.performed -= Back;
        };
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
