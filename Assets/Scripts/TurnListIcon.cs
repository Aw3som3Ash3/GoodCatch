using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnListIcon : VisualElement
{
    VisualElement icon;
    

    public new class UxmlFactory : UxmlFactory<TurnListIcon, TurnListIcon.UxmlTraits>
    {

    }
    public TurnListIcon()
    {
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/TurnListIcon.uxml");

        TemplateContainer ui = visualTreeAsset.Instantiate();

        Add(ui);
    }
    public TurnListIcon(Texture2D image,CombatManager.Team team)
    {
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/TurnListIcon.uxml");
        visualTreeAsset.CloneTree(this);
        icon = this.Q("Icon");
        if (image != null)
        {
            icon.style.backgroundImage = image;
        }
        var color=team==CombatManager.Team.player? Color.green: Color.red;
        icon.style.borderRightColor=color;
        icon.style.borderLeftColor=color;
        icon.style.borderTopColor=color;
        icon.style.borderBottomColor=color;
        
    }
}
