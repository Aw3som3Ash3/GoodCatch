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
    Label itemName, ownedAmount, detailText;
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
        this.focusable = true;
        this.delegatesFocus = true;
        detailText = this.Q<Label>("DetailText");
        itemName = this.Q<Label>("ItemName");
        ownedAmount = this.Q<Label>("OwnedAmount");
        inventoryTabs.onSelectionChange += OnSelectionChange;


    }

    private void OnSelectionChange(IEnumerable<object> enumerable)
    {
        ItemInventory.ItemSlot item = enumerable.FirstOrDefault() as ItemInventory.ItemSlot;
        Debug.Log("item selected: " + item?.Item.name);
        detailText.text = item?.Item.Description;
        itemName.text = item?.Item.name;
        ownedAmount.text = item?.amount.ToString();
    }

    public override bool Back()
    {
        InputManager.Input.UI.ChangeTab.performed -= ChangeTab;
        InputManager.Input.UI.ChangeTab.Disable();
        return base.Back();
        
    }


    void ChangeTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            inventoryTabs.ChangeTab((int)context.ReadValue<float>());
            Debug.Log("tab change: " + (int)context.ReadValue<float>());


        }

    }


    
}
