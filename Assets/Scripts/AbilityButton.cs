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

    Label title,damage;

    public event Action<Action<AbilityToolTip>> MouseEnter;
    public event Action MouseExit;

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
        this.RegisterCallback<MouseOutEvent>((x) => {MouseExit?.Invoke(); });
        title=new Label();
        damage=new Label();
        
    }

    void PopulateToolTip(AbilityToolTip element)
    {
        if (element.EnableToolTip(this))
        {
            element.content.Clear();
            element.content.Add(title);
            element.content.Add(damage);
            
        }
       
    }

    public void SetAbility(Ability ability,float damage) 
    {
        abilityName = ability.name;
        text = ability.name;
        title.text = abilityName;
        this.damage.text = damage.ToString();

    }
}
