using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityButton : Button
{

    //Button button;
    string abilityName;
    AbilityToolTipTitle title;
    AbilityTooltipActions damageLabel;
    List<AbilityTooltipStatusChance> effectLabels;
    VisualElement buttonCover;

   // Clickable clickable;
    
    bool usable;
    public event Action<Action<ToolTipBox>> MouseEnter;
    public event Action MouseExit;
    //public event Action clicked;
    const string PLAYER_TAB_CLASS = "PlayerColor";
    const string TARGET_ENEMY_TAB_CLASS = "TargetEnemy";
    const string TARGET_TEAM_TAB_CLASS = "TargetTeam";
    const string USABLE_DEPTH_TAB_CLASS = "UsableDepth";
    const string TARGETABLE_DEPTH_TAB_CLASS = "TargetableDepth";
    VisualElement leftColumn,rightColumn;
    VisualElement[] leftTabs,rightTabs;
    
    public new class UxmlFactory : UxmlFactory<AbilityButton, AbilityButton.UxmlTraits>
    {

    }
    // Start is called before the first frame update
    public AbilityButton()
    {

        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/AbilityButton");
        visualTreeAsset.CloneTree(root);
        this.focusable = true;
        this.delegatesFocus = true;
        this.pickingMode = PickingMode.Ignore;
        this.RegisterCallback<MouseEnterEvent>((x) => {MouseEnter?.Invoke(PopulateToolTip); });
        this.RegisterCallback<FocusInEvent>((x) => {MouseEnter?.Invoke(PopulateToolTip); });
       
        this.RegisterCallback<MouseOutEvent>((x) => {MouseExit?.Invoke(); });
        this.RegisterCallback<FocusOutEvent>((x) => {MouseExit?.Invoke(); });
        leftColumn = this.Q<VisualElement>("LeftColumn");
        rightColumn = this.Q<VisualElement>("RightColumn");
        leftTabs = leftColumn.Children().ToArray();
        rightTabs = rightColumn.Children().ToArray();
        for (int i = 0; i < 3; i++)
        {
            leftTabs[i].AddToClassList(USABLE_DEPTH_TAB_CLASS);
            rightTabs[i].AddToClassList(TARGETABLE_DEPTH_TAB_CLASS);
            rightTabs[i].AddToClassList(TARGET_ENEMY_TAB_CLASS);
            

        }
        tabIndex =1;
        title=new AbilityToolTipTitle();
        damageLabel=new AbilityTooltipActions();
        effectLabels = new();
        buttonCover = this.Q<VisualElement>("ButtonCover");
        //clickable =new Clickable(() => 
        //{ 
        //    if (usable) 
        //    { 
        //        clicked?.Invoke(); 
        //    } 
        //});
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
    public void SetAbility(Ability ability,float damage,int baseAccuracy) 
    {
        //for (int i = 0; i < 3; i++)
        //{
        //    leftTabs[i].RemoveFromClassList(USABLE_DEPTH_TAB_CLASS);
        //    rightTabs[i].RemoveFromClassList(TARGETABLE_DEPTH_TAB_CLASS);
            

        //}
        for (int i = 0; i < 3; i++)
        {
            if (ability.AvailableDepths.HasFlag((Depth)(1 << i)))
            {
                leftTabs[i].AddToClassList(USABLE_DEPTH_TAB_CLASS);
            }
            else
            {
                leftTabs[i].RemoveFromClassList(USABLE_DEPTH_TAB_CLASS);
            }

            if (ability.TargetableDepths.HasFlag((Depth)(1 << i)))
            {
                rightTabs[i].AddToClassList(TARGETABLE_DEPTH_TAB_CLASS);
            }
            else
            {
                rightTabs[i].RemoveFromClassList(TARGETABLE_DEPTH_TAB_CLASS);
            }

        }
        if (ability.TargetedTeam == Ability.TargetTeam.enemy)
        {
            rightTabs[0].AddToClassList(TARGET_ENEMY_TAB_CLASS);
            rightTabs[0].RemoveFromClassList(TARGET_TEAM_TAB_CLASS);

            rightTabs[1].AddToClassList(TARGET_ENEMY_TAB_CLASS);
            rightTabs[1].RemoveFromClassList(TARGET_TEAM_TAB_CLASS);

            rightTabs[2].AddToClassList(TARGET_ENEMY_TAB_CLASS);
            rightTabs[2].RemoveFromClassList(TARGET_TEAM_TAB_CLASS);
        }
        else if (ability.TargetedTeam == Ability.TargetTeam.friendly)
        {
            rightTabs[0].AddToClassList(TARGET_TEAM_TAB_CLASS);
            rightTabs[0].RemoveFromClassList(TARGET_ENEMY_TAB_CLASS);

            rightTabs[1].AddToClassList(TARGET_TEAM_TAB_CLASS);
            rightTabs[1].RemoveFromClassList(TARGET_ENEMY_TAB_CLASS);

            rightTabs[2].AddToClassList(TARGET_TEAM_TAB_CLASS);
            rightTabs[2].RemoveFromClassList(TARGET_ENEMY_TAB_CLASS);

        }
        abilityName = ability.name;
        title.SetToolTip(abilityName,"",ability.AvailableDepths,ability.TargetableDepths);
        var backgroundValue=buttonCover.style.backgroundImage.value;
        backgroundValue.sprite= ability.Icon;
        buttonCover.style.backgroundImage=backgroundValue;
        //text = ability.name;
        this.damageLabel.SetDamage(damage, ability.Accuracy + baseAccuracy*0.01f);
        effectLabels.Clear();
        for (int i = 0;i<ability.Effects.Length;i++) 
        {
            var label = new AbilityTooltipStatusChance();
            effectLabels.Add(label);
            label.SetStatusEffect(ability.Effects[i].Effect, ability.Effects[i].Chance);
        }
        
    }
}
