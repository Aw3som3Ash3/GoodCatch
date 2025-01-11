using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityToolTipTitle : TooltipModule
{
    public Label Title { get; private set; }
    public Label Description { get; private set; }
    VisualElement[] usageDepths=new VisualElement[3];
    VisualElement[] targetableDepths=new VisualElement[3];

    public new class UxmlFactory : UxmlFactory<AbilityToolTipTitle, AbilityToolTipTitle.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public AbilityToolTipTitle():base("AbilityTitle")
    {

    }
    protected override void Init()
    {
        Title = this.Q<Label>("Title");
        Description = this.Q<Label>("Description");
        for (int i = 0; i < 3; i++)
        {
            usageDepths[i] = this.Q("UsableDepths").Q("dot" + (i + 1));
            targetableDepths[i] = this.Q("TargetableDepths").Q("dot" + (i + 1));
        }
        
    }
    public void SetToolTip(string name, string desctiption, Depth usableDepth, Depth targetableDepth)
    {
        Title.text= name;
        SetDepthIcons(usableDepth, targetableDepth);
    }

    void SetDepthIcons(Depth usableDepth, Depth targetableDepth)
    {

        for (int i = 0; i < 3; i++)
        {
            if (usableDepth.HasFlag((Depth)(1 << i)))
            {
                usageDepths[i].style.unityBackgroundImageTintColor = Color.cyan;
            }
            else
            {
                usageDepths[i].style.unityBackgroundImageTintColor = Color.grey;
            }

            if (targetableDepth.HasFlag((Depth)(1 << i)))
            {
                targetableDepths[i].style.unityBackgroundImageTintColor = Color.red;
            }
            else
            {
                targetableDepths[i].style.unityBackgroundImageTintColor = Color.grey;
            }

        }
    }

    
}

public abstract class TooltipModule : VisualElement
{

    
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public TooltipModule()
    {

    }
    public TooltipModule(string name)
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>($"UXMLs/{name}");

        visualTreeAsset.CloneTree(root);
        
        this.style.flexGrow = 1;
        var widthVal = this.style.width.value;
        widthVal = Length.Percent(100);
        this.style.width = widthVal;
        Init();
    }
    abstract protected void Init();


}

public class AbilityTooltipActions : TooltipModule
{
    Label damageAmount, accuracyAmount;
    Label damageTitle;
    public float damage { get; private set; }
    //public float accuracy; 
    public AbilityTooltipActions():base("AbilityAction")
    {

    }

    public void SetDamage(float damage,float accuracy)
    {
        damageTitle.text = damage > 0 ? "<color=red> Damage: </color>" : "<color=green> Healing: </color>";
        damageAmount.text=Mathf.Abs(damage).ToString();
        accuracyAmount.text = ((int)(accuracy *100)).ToString()+"%";
        this.damage = damage;
    }
    protected override void Init()
    {

        damageAmount = this.Q<Label>("DamageHealthAmount");
        damageTitle = this.Q<Label>("DamageHealth");
        accuracyAmount = this.Q<Label>("AccuracyAmount");

    }
}


public class AbilityTooltipStatusChance : TooltipModule
{
    Label statusName,chance,effect;
    public AbilityTooltipStatusChance() : base("AbilityStatus")
    {

    }
    public void SetStatusEffect(StatusEffect effect,float chance)
    {
        statusName.text = effect.name;
        this.chance.text = (chance * 100).ToString() + "%";
    }
    protected override void Init()
    {
        statusName = this.Q<Label>("StatusName");
        chance = this.Q<Label>("StatusChancePercentageAmount");

        //throw new System.NotImplementedException();
    }
}



public class StatusEffectToolTip : TooltipModule
{
    Label statusName, roundsRemaining, description;
    public StatusEffectToolTip():base("ToolTipStatusActive")
    {

    }
    public void SetSatus(StatusEffect.StatusEffectInstance effect)
    {
        statusName.text = effect.effect.name;
        roundsRemaining.text = effect.remainingDuration.ToString();
    }
    protected override void Init()
    {
        statusName = this.Q<Label>("StatusName");
        roundsRemaining = this.Q<Label>("StatusRoundsAmount");
        //throw new System.NotImplementedException();
    }
}
