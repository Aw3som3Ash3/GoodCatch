using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityButton : Button
{

    Button button;
    string abilityName;
    AbilityToolTipTitle title;
    AbilityTooltipActions damageLabel;
    List<AbilityTooltipStatusChance> effectLabels;

    bool usable;
    public event Action<Action<ToolTipBox>> MouseEnter;
    public event Action MouseExit;
    public new event Action clicked;
    public new class UxmlFactory : UxmlFactory<AbilityButton, AbilityButton.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    // Start is called before the first frame update
    public AbilityButton()
    {
        button = this.Q<Button>();
        
        this.RegisterCallback<MouseEnterEvent>((x) => {MouseEnter?.Invoke(PopulateToolTip); });
        this.RegisterCallback<FocusInEvent>((x) => {MouseEnter?.Invoke(PopulateToolTip); });
       
        this.RegisterCallback<MouseOutEvent>((x) => {MouseExit?.Invoke(); });
        this.RegisterCallback<FocusOutEvent>((x) => {MouseExit?.Invoke(); });
        tabIndex=1;
        title=new AbilityToolTipTitle();
        damageLabel=new AbilityTooltipActions();
        effectLabels = new();
        base.clicked += () => { if (usable) { clicked?.Invoke(); } };
    }

    void PopulateToolTip(ToolTipBox element)
    {
        if (abilityName == null)
        {
            return;
        }
        if (element.EnableToolTip(this))
        {
            element.content.Clear();
            element.content.Add(title);
            if (damageLabel.damage != 0)
            {
                element.content.Add(damageLabel);
            }
            for (int i = 0; i < effectLabels.Count; i++)
            {
                element.content.Add(effectLabels[i]);
            }
           
           
            
        }
       
    }
    public void SetUsability(bool b)
    {
        usable = b;
        if(b)
        {
            this.RemoveFromClassList("unity-disabled");
        }
        else
        {
            this.AddToClassList("unity-disabled");
           
        }
        
        
    }
    public void SetPreview(bool b)
    {
        if (b)
        {
            this.RemoveFromClassList("unity-disabled");
        }
        else
        {
            this.AddToClassList("unity-disabled");

        }


    }
    public void SetAbility(Ability ability,float damage,float baseAccuracy) 
    {
       
        abilityName = ability.name;
        title.SetToolTip(abilityName,"",ability.AvailableDepths,ability.TargetableDepths);
        text = ability.name;
        this.damageLabel.SetDamage(damage, ability.Accuracy + baseAccuracy);
        effectLabels.Clear();
        for (int i = 0;i<effectLabels.Count;i++) 
        {
            var label = new AbilityTooltipStatusChance();
            effectLabels.Add(label);
            label.SetStatusEffect(ability.Effects[i].Effect, ability.Effects[i].Chance);
        }
        
    }
}
