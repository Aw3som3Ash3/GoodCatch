using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : VisualElement
{
    Button settting, party, beastiary, inventory;
    CursorLockMode prevMode;
    bool prevVisability;
    public new class UxmlFactory : UxmlFactory<PauseMenu, CombatUI.UxmlTraits>
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
        this.SetEnabled(false);
        this.visible = false;
        this.style.flexGrow = 1;
        beastiary=this.Q<Button>("FishBookButton");
        party = this.Q<Button>("PartyButton");
        settting = this.Q<Button>("OptionsButton");
        inventory = this.Q<Button>("ItemsButton");
        party.clicked += () =>throw new NotImplementedException();
        beastiary.clicked += () =>throw new NotImplementedException();
        inventory.clicked += () =>throw new NotImplementedException();
        settting.clicked += () =>throw new NotImplementedException();
        InputManager.Input.UI.Pause.Enable();
        InputManager.Input.UI.Pause.performed += OnPause;

    }

    void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!this.enabledSelf)
            {
                prevVisability = UnityEngine.Cursor.visible;
                prevMode = UnityEngine.Cursor.lockState;

            }
            this.SetEnabled(!this.enabledSelf);
            this.visible = enabledSelf;
            this.BringToFront();
            UnityEngine.Cursor.lockState = this.enabledSelf ? CursorLockMode.Confined: prevMode;
            UnityEngine.Cursor.visible = this.enabledSelf ? true: prevVisability;
            Time.timeScale = this.enabledSelf ? 0: 1;
            if (this.enabledSelf)
            {
                InputManager.DisablePlayer();
            }
            else
            {
                InputManager.EnablePlayer();
            }
            

        }
       
    }
}
