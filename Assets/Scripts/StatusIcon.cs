using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StatusIcon : VisualElement
{
    public Action<Action<AbilityToolTip>> mouseEnter;
    public Action mouseOut;
    VisualElement imageObj;
    [SerializeField]
    Label turnsLeftText;
    StatusEffect.StatusEffectInstance statusEffect;
    Label effectDescription= new Label();
    public new class UxmlFactory : UxmlFactory<StatusIcon, StatusIcon.UxmlTraits>
    {

    }
    public StatusIcon()
    {
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/StatusIcon.uxml");

        visualTreeAsset.CloneTree(this);

    }
    public StatusIcon(StatusEffect.StatusEffectInstance statusEffect)
    {
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/StatusIcon.uxml");

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
        this.RegisterCallback<MouseOverEvent>((x) => mouseEnter?.Invoke(PopulateToolTip));
        this.RegisterCallback<MouseOutEvent>((x) => mouseOut?.Invoke());
        
        effectDescription.text = "nothing for now";

    }
    void PopulateToolTip(AbilityToolTip element)
    {
        element.content.Clear();
        element.content.Add(effectDescription);
        element.EnableToolTip(this);
    }
    public void UpdateIcon(int duration)
    {
        turnsLeftText.text = duration.ToString();
    }
}
