using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatItemUI : VisualElement
{
    int amount;
    public Item item { get; private set; }
    Button button;
    public Action<Item> Clicked;

    public event Action<Action<ToolTipBox>> MouseEnter;
    public event Action MouseExit;
    //public event Action OnDestroy;

    public new class UxmlFactory : UxmlFactory<CombatItemUI, CombatItemUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public CombatItemUI()
    {
        VisualElement root = this;

        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/CombatItem");


        visualTreeAsset.CloneTree(root);
        
    }
    public CombatItemUI(Item item,int amount)
    {
        VisualElement root = this;

        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/CombatItem");


        visualTreeAsset.CloneTree(root);
        this.amount = amount;
        this.item = item;
        button = this.Q<Button>();
        if (item.Icon != null)
        {
            button.style.backgroundImage = item.Icon;
        }
        this.RegisterCallback<MouseOverEvent>((X) => MouseEnter?.Invoke(PopulateToolTip));
        this.RegisterCallback<FocusInEvent>((X) => MouseEnter?.Invoke(PopulateToolTip));
        this.RegisterCallback<MouseOutEvent>((X) => MouseExit?.Invoke());
        this.RegisterCallback<FocusOutEvent>((X) => MouseExit?.Invoke());
        button.text = item.name;
        button.Q<Label>("Amount").text = amount.ToString();
        button.clicked +=()=> Clicked(this.item);

    }
    public void SetAmount(int amount)
    {
        this.amount = amount;
        button.Q<Label>("Amount").text = amount.ToString();
    }

    protected void PopulateToolTip(ToolTipBox element)
    {
        if (element.EnableToolTip(this))
        {
            element.content.Clear();
        }
       
    }
    ~CombatItemUI()
    {
        MouseExit?.Invoke();
    }
}
