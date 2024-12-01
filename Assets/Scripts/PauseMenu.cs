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
    PartyUI partyUI;
    OptionsPage optionsPage;
    Bestiary bestiaryPage;
    PausePage currentPage;
    VisualElement menu;

    static PauseMenu mainPause;
    public new class UxmlFactory : UxmlFactory<PauseMenu, PauseMenu.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public PauseMenu()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/PauseMenu.uxml");
        visualTreeAsset.CloneTree(root);
       
        this.style.flexGrow = 1;
        this.style.position = Position.Absolute;
        this.StretchToParentSize();

        bestiary =this.Q<Button>("FishBookButton");
        party = this.Q<Button>("PartyButton");
        settting = this.Q<Button>("OptionsButton");
        inventory = this.Q<Button>("ItemsButton");
        menu = this.Q("PauseMenuWorkSpace");
        party.clicked +=Party;
        bestiary.clicked += () => BestiaryScreen();
        inventory.clicked += () =>throw new NotImplementedException();
        settting.clicked += Options;


        mainPause = this;
        Debug.Log(this);

        menu.focusable = true;

        //this.delegatesFocus = true;
        menu.SetEnabled(false);
        menu.visible = false;
        menu.RegisterCallback<NavigationMoveEvent>(OnNavigate);
    }

    void OnNavigate(NavigationMoveEvent evt)
    {
       

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
    static public void Pause()
    {
        mainPause.OnPause();
    }
    void OnPause()
    {

        //Debug.Log(party.focusController.focusedElement);
        Debug.Log("pausing");
        if (currentPage != null)
        {
            Back();
            return;
        }
        if (!menu.enabledSelf)
        {
            prevVisability = UnityEngine.Cursor.visible;
            prevMode = UnityEngine.Cursor.lockState;

        }
        menu.SetEnabled(!menu.enabledSelf);
        menu.visible = menu.enabledSelf;
        //this.BringToFront();

        Time.timeScale = menu.enabledSelf ? 0 : 1;
        if (menu.enabledSelf)
        {
            menu.Focus();
            //this.Focus();
            //this.CaptureMouse();
            //this.pickingMode =;
            if (GameManager.Instance.inputMethod == InputMethod.controller)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Confined;
                UnityEngine.Cursor.visible = true;

            }

            InputManager.DisablePlayer();
            InputManager.Input.UI.Back.Enable();
            InputManager.Input.UI.Back.performed += Back;

        }
        else
        {
            UnityEngine.Cursor.lockState = prevMode;
            UnityEngine.Cursor.visible = prevVisability;
            InputManager.EnablePlayer();
            InputManager.Input.UI.Back.Disable();
            InputManager.Input.UI.Back.performed -= Back;
        }

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
            }
        }

    }

    
    void Options()
    {
        if (optionsPage == null)
        {
            optionsPage = new();

        }
        this.Add(optionsPage);
        menu.visible = false;
        menu.SetEnabled(false);
        optionsPage.OpenOptions();
        currentPage = optionsPage;
    }
    void Party()
    {
        if (partyUI == null)
        {
            partyUI = new();

        }

        this.Add(partyUI);
        partyUI.UpdateUI();
        menu.visible = false;
        menu.SetEnabled(false);
        currentPage =partyUI;
    }


    void BestiaryScreen()
    {
        if (bestiaryPage == null)
        {
            bestiaryPage = new();
        }

        this.Add(bestiaryPage);
        menu.visible = false;
        menu.SetEnabled(false);
        currentPage = bestiaryPage;
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