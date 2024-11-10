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
    public new class UxmlFactory : UxmlFactory<CombatItemUI, CombatItemUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public CombatItemUI()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/CombatItem.uxml");

        visualTreeAsset.CloneTree(root);
        
    }
    public CombatItemUI(Item item,int amount)
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/CombatItem.uxml");

        visualTreeAsset.CloneTree(root);
        this.amount = amount;
        this.item = item;
        button = this.Q<Button>();
        if (item.Icon != null)
        {
            button.style.backgroundImage = item.Icon;
        }
        button.text = item.name;
        button.Q<Label>("Amount").text = amount.ToString();
        button.clicked +=()=> Clicked(this.item);

    }
    public void SetAmount(int amount)
    {
        this.amount = amount;
        button.Q<Label>("Amount").text = amount.ToString();
    }
}
