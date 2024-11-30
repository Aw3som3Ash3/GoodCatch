using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        mainScreen.Q<Button>("LoadGame").clicked += () =>
        {
            mainScreen.visible=false;
            loadScreen.visible=true;
        };

        for(int i=1;i<=3; i++)
        {
            int index = i;
            var button= loadScreen.Q<Button>("Slot" + i);
            button.clicked += () => { SavingSystem.LoadGame(index); };
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
