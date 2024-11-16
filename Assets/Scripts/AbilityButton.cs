using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityButton : VisualElement
{
    public Action mouseOver, mouseExit,clicked;
    VisualElement tooltipElement;
    Button button;
    string abilityName;
    public new class UxmlFactory : UxmlFactory<AbilityButton, AbilityButton.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    // Start is called before the first frame update
    public AbilityButton()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/AbilityButton.uxml");
        visualTreeAsset.CloneTree(root);
        tooltipElement = this.Q("tooltip");
        button = this.Q<Button>();
        tooltipElement.visible = false;
        
        button.RegisterCallback<MouseOverEvent>((x) => {  tooltipElement.visible = true; tooltipElement.Focus(); mouseOver?.Invoke(); });
        button.RegisterCallback<MouseOutEvent>((x) => { tooltipElement.visible = false; mouseExit?.Invoke(); });

        button.clicked+=()=> clicked?.Invoke();
        
    }
    public void SetAbility(Ability ability) 
    {
        abilityName = ability.name;
        button.text = ability.name;

    }



}
