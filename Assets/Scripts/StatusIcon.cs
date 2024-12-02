using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StatusIcon : VisualElement
{
    VisualElement imageObj;
    [SerializeField]
    Label turnsLeftText;
    StatusEffect.StatusEffectInstance statusEffect;
    StatusEffectToolTip effectDescription= new StatusEffectToolTip();

    public event Action<Action<ToolTipBox>> MouseEnter;
    public event Action MouseExit;

    public new class UxmlFactory : UxmlFactory<StatusIcon, StatusIcon.UxmlTraits>
    {

    }
    public StatusIcon()
    {
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/StatusIcon");


        visualTreeAsset.CloneTree(this);

    }
    public StatusIcon(StatusEffect.StatusEffectInstance statusEffect)
    {
       
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/StatusIcon");


        visualTreeAsset.CloneTree(this);
        imageObj=this.Q("Icon");
        turnsLeftText = this.Q<Label>("Duration");
        this.statusEffect = statusEffect;
        if (statusEffect.effect.Icon != null)
        {
            imageObj.style.backgroundImage = this.statusEffect.effect.Icon;
        }
        UpdateIcon(statusEffect.remainingDuration);
        this.statusEffect.DurationChanged += UpdateIcon;
        this.RegisterCallback<MouseOverEvent>((x) => MouseEnter?.Invoke(PopulateToolTip));
        this.RegisterCallback<FocusInEvent>((x) => MouseEnter?.Invoke(PopulateToolTip));
        this.RegisterCallback<MouseOutEvent>((x) => MouseExit?.Invoke());
        this.RegisterCallback<FocusOutEvent>((x) => MouseExit?.Invoke());
        
        effectDescription.SetSatus(statusEffect);

    }
    void PopulateToolTip(ToolTipBox element)
    {
        if (element.EnableToolTip(this))
        {
            element.content.Clear();
            element.content.Add(effectDescription);
            element.EnableToolTip(this);
        }
           
    }
    public void UpdateIcon(int duration)
    {
        turnsLeftText.text = duration.ToString();
    }

}
