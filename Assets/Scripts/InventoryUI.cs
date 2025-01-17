using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InventoryUI : PausePage
{
    VisualElement leftTab, rightTab;
    InventoryTabs inventoryTabs;
    public new class UxmlFactory : UxmlFactory<InventoryUI, InventoryUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }

    public InventoryUI()
    {
        Init();
    }
    
    void Init()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/NewInventory");

        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        inventoryTabs = this.Q<InventoryTabs>();
        InputManager.Input.UI.ChangeTab.performed += ChangeTab;
        InputManager.Input.UI.ChangeTab.Enable();
        leftTab = this.Q("LeftBumper");
        rightTab = this.Q("RightBumper");
        ChangeIcons(GameManager.Instance.inputMethod);
        GameManager.Instance.OnInputChange += OnInputChange;

    }
    public override bool Back()
    {
        InputManager.Input.UI.ChangeTab.performed -= ChangeTab;
        InputManager.Input.UI.ChangeTab.Disable();
        return base.Back();
        
    }
    private void OnInputChange(InputMethod method)
    {
        ChangeIcons(method);
    }

    void ChangeTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            inventoryTabs.ChangeTab((int)context.ReadValue<float>());
            Debug.Log("tab change: " + (int)context.ReadValue<float>());


        }

    }

    void ChangeIcons(InputMethod inputMethod)
    {
        if (inputMethod == InputMethod.mouseAndKeyboard)
        {
            
            
            
            
            
            
        }
        foreach (var binding in InputManager.Input.UI.ChangeTab.bindings)
        {
            if (binding.isPartOfComposite && binding.groups == "Keyboard&Mouse" && inputMethod == InputMethod.mouseAndKeyboard)
            {
                if (binding.name == "positive")
                {
                    rightTab.style.backgroundImage = Resources.Load<Texture2D>("ControlIcons/Keyboard & Mouse/Default/keyboard_" + binding.path.Replace("<Keyboard>/", ""));

                    Debug.Log("has binding " + binding.effectivePath);

                }
                else if (binding.name == "negative")
                {
                    leftTab.style.backgroundImage = Resources.Load<Texture2D>("ControlIcons/Keyboard & Mouse/Default/keyboard_" + binding.path.Replace("<Keyboard>/", ""));
                }
            }
            else if (binding.isPartOfComposite && binding.groups == "Gamepad" && inputMethod == InputMethod.controller)
            {
                if (binding.name == "positive")
                {
                    rightTab.style.backgroundImage = Resources.Load<Texture2D>("ControlIcons/Xbox Series/Default/xbox_" + binding.path.Replace("<Gamepad>/", ""));

                    Debug.Log("has binding " + binding.effectivePath);

                }
                else if (binding.name == "negative")
                {

                    leftTab.style.backgroundImage = Resources.Load<Texture2D>("ControlIcons/Xbox Series/Default/xbox_" + binding.path.Replace("<Gamepad>/", ""));
                }
            }



        }

    }
}
