using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : VisualElement
{
    Button settting, party, bestiary, inventory;
    CursorLockMode prevMode;
    bool prevVisability;
    Dictionary<InputAction, bool> inputsPrevEnabled=new();
    //PartyUI partyUI;
    //OptionsPage optionsPage;
    //Bestiary bestiaryPage;
    PausePage currentPage;
    //InventoryUI inventoryUI;
    VisualElement menu;

    VisualElement lastSelected;

    bool exitCompletely = false;
    static PauseMenu mainPause;
    static public event Action<bool> GamePaused;

    public static bool PauseActive { get { return mainPause.menu.visible; } }

    public new class UxmlFactory : UxmlFactory<PauseMenu, PauseMenu.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public PauseMenu()
    {
        if (mainPause != null)
        {
            InputManager.Input.UI.Back.Disable();
            InputManager.Input.UI.Back.performed -= mainPause.Back;
            InputManager.OnInputChange -= mainPause.OnInputChanged;
        }

        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/PauseMenu");

        visualTreeAsset.CloneTree(root);
       
        this.style.flexGrow = 1;
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        this.pickingMode = PickingMode.Ignore;
        bestiary =this.Q<Button>("FishBookButton");
        party = this.Q<Button>("PartyButton");
        settting = this.Q<Button>("OptionsButton");
        inventory = this.Q<Button>("ItemsButton");
        menu = this.Q("PauseMenuWorkSpace");
        party.clicked +=Party;
        bestiary.clicked += () => BestiaryScreen();
        inventory.clicked += () =>InventoryMenu();
        settting.clicked += Options;


        mainPause = this;
        Debug.Log(this);

        this.focusable = true;
        
        //this.delegatesFocus = true;

        menu.SetEnabled(false);
        menu.visible = false;
        mainPause.SetEnabled(false);
        this.RegisterCallback<NavigationMoveEvent>(OnNavigate);
        
    }
    public static void ClearPause()
    {
        mainPause = null;
    }
    private void OnInputChanged(InputMethod method)
    {
        if (method == InputMethod.controller)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;

        }
    }

    void OnNavigate(NavigationMoveEvent evt)
    {

        if (menu.visible)
        {
            Debug.Log("overriding navigate");
            switch (evt.direction)
            {
                case NavigationMoveEvent.Direction.Left:
                    party.Focus();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    inventory.Focus();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    bestiary.Focus();
                    break;
                case NavigationMoveEvent.Direction.Down:
                    settting.Focus();
                    break;

            }
            evt.PreventDefault();
        }
        

        
    }
    static public PauseMenu Pause()
    {

        if (mainPause != null && mainPause.menu.visible == false && mainPause.menu.panel!=null)
        {
            mainPause.OnPause();
        }
        else
        {
            return null;
        }
        
        return mainPause;
    }
    public void AddPage(PausePage pausePage, bool exitCompletely = true)
    {
       
        this.Add(pausePage);
        currentPage = pausePage;
        menu.visible = false;
        menu.SetEnabled(false);
        this.exitCompletely = exitCompletely;

    }
    void OnPause()
    {
        //if (!GameManager.Instance.canPause)
        //{
        //    return;
        //}
        
        //Debug.Log(party.focusController.focusedElement);
        Debug.Log("pausing");
        if (currentPage != null)
        {
            Back();
            
            return;
        }
        
        exitCompletely = false;
        if (!menu.enabledSelf)
        {
            prevVisability = UnityEngine.Cursor.visible;
            prevMode = UnityEngine.Cursor.lockState;
            
            foreach(InputAction inputAction in InputManager.Input)
            {
                inputsPrevEnabled[inputAction]=inputAction.enabled;
            }
            
            //playerControlsEnabled=InputManager.Input.Player.enabled;

        }
        mainPause.SetEnabled(!mainPause.enabledSelf);
        menu.SetEnabled(mainPause.enabledSelf);
        menu.visible = menu.enabledSelf;
        //this.BringToFront();

        Time.timeScale = menu.enabledSelf ? 0 : 1;
        if (menu.enabledSelf)
        {
            InputManager.OnInputChange += OnInputChanged;
            this.Focus();
            //party.Focus();
            //this.Focus();
            //this.CaptureMouse();
            //this.pickingMode =;
            if (InputManager.inputMethod == InputMethod.controller)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Confined;
                UnityEngine.Cursor.visible = true;

            }
            InputManager.Input.Ship.Disable();
            InputManager.Input.Fishing.Disable();
            InputManager.DisablePlayer();
            InputManager.Input.UI.Back.Enable();
            InputManager.Input.UI.Back.performed += Back;
            GameManager.Instance.canPause = false;
            

        }
        else
        {
            UnityEngine.Cursor.lockState = prevMode;
            UnityEngine.Cursor.visible = prevVisability;
            foreach (InputAction inputAction in InputManager.Input)
            {
                if (inputsPrevEnabled[inputAction])
                {
                    inputAction.Enable();
                }
                
            }

            InputManager.Input.UI.Back.Disable();
            InputManager.Input.UI.Back.performed -= Back;
            InputManager.OnInputChange -= OnInputChanged;
            GameManager.Instance.canPause = true;
        }
        if (panel != null)
        {
            var bottomMapping = panel.visualTree.Q("BottomMapping");
            if (bottomMapping != null)
            {
                bottomMapping.visible = !PauseActive;
            }
            var questUI = panel.visualTree.Q("QuestUI");
            if (questUI != null)
            {
                questUI.visible = !PauseActive;
            }
        }
        
        GamePaused?.Invoke(menu.enabledSelf);
    }
   
    void Back(InputAction.CallbackContext context=default)
    {
        if (currentPage != null)
        {
            if (currentPage.Back())
            {
                this.Remove(currentPage);

                menu.SetEnabled(true);
                menu.visible = true;
                currentPage = null;
                this.delegatesFocus = false;
                this.Focus();
                //lastSelected?.Focus();
                Debug.Log("last selected: " + lastSelected);

            }
           
            if (exitCompletely)
            {
                OnPause();
            }
        }
        else
        {
            OnPause();
        }



    }
    void InventoryMenu()
    {

        InventoryUI inventoryUI = new();
        lastSelected = inventory;
        this.Add(inventoryUI);
        //partyUI.UpdateUI();
        menu.visible = false;
        menu.SetEnabled(false);
        inventory.Focus();
        currentPage = inventoryUI;
        this.delegatesFocus = true;
    }
    
    void Options()
    {
        OptionsPage optionsPage = new();
        lastSelected = optionsPage;
        this.Add(optionsPage);
        menu.visible = false;
        menu.SetEnabled(false);
        optionsPage.OpenOptions();
        optionsPage.Focus();
        currentPage = optionsPage;
        this.delegatesFocus = true;
    }
    void Party()
    {
        lastSelected = party;
        NewPartyUI partyUI = new();
        this.Add(partyUI);
        partyUI.UpdateUI();
        menu.visible = false;
        menu.SetEnabled(false);
        //partyUI.Focus();
        currentPage =partyUI;
        this.delegatesFocus = true;
    }


    void BestiaryScreen()
    {

        Bestiary bestiaryPage = new();
        lastSelected = bestiary;
        this.Add(bestiaryPage);
        menu.visible = false;
        bestiaryPage.Focus();
        menu.SetEnabled(false);
        currentPage = bestiaryPage;
        this.delegatesFocus = true;
    }

    
    ~PauseMenu()
    {
        
    }
    
}


public abstract class PausePage : VisualElement
{
    public virtual bool Back()
    {
        return true;
        
        //this.BringToFront();


    }
}