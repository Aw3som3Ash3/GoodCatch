using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityButton : Button
{
    public Action<Action<AbilityToolTip>> mouseEnter;
    public Action mouseExit;

    Button button;
    string abilityName;

    Label title,damage;
    public new class UxmlFactory : UxmlFactory<AbilityButton, AbilityButton.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    // Start is called before the first frame update
    public AbilityButton()
    {
        //VisualElement root = this;
        //VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/AbilityButton.uxml");
        //visualTreeAsset.CloneTree(root);
        //tooltipElement = this.Q("tooltip");
        button = this.Q<Button>();
        //tooltipElement.visible = false;
        
        this.RegisterCallback<MouseEnterEvent>((x) => {  mouseEnter?.Invoke(PopulateToolTip); });
        this.RegisterCallback<MouseOutEvent>((x) => { mouseExit?.Invoke(); });
        title=new Label();
        damage=new Label();
        //button.clicked+=()=> clicked?.Invoke();
        
    }

    private void PopulateToolTip(AbilityToolTip element)
    {
        
        element.content.Clear();
        element.content.Add(title);
        element.content.Add(damage);
        element.EnableToolTip(this);
    }

    public void SetAbility(Ability ability,float damage) 
    {
        abilityName = ability.name;
        text = ability.name;
        title.text = abilityName;
        this.damage.text = damage.ToString();

    }

    



}
