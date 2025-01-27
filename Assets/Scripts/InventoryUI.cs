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
        foreach (var binding in InputManager.Input.UI.ChangeTab.bindings.Where((x)=>x.groups== (inputMethod== InputMethod.mouseAndKeyboard?"Keyboard&Mouse":"Gamepad")))
        {
            if (binding.isPartOfComposite)
            {
                if (binding.name == "positive")
                {
                    InputDisplayer.GetInputIcon(binding, inputMethod).Completed+=(x) => rightTab.style.backgroundImage = x.Result;


                    Debug.Log("has binding " + binding.effectivePath);

                }
                else if (binding.name == "negative")
                {
                    InputDisplayer.GetInputIcon(binding, inputMethod).Completed += (x) => leftTab.style.backgroundImage = x.Result;
                }
            }
            



        }

    }
}
