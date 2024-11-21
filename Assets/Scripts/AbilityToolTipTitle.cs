using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityToolTipTitle : VisualElement
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
    public AbilityToolTipTitle()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/AbilityTitle.uxml");
        visualTreeAsset.CloneTree(root);
        Title = this.Q<Label>("Title");
        Description = this.Q<Label>("Description");
        this.style.flexGrow = 1;
        var widthVal = this.style.width.value;
        widthVal=Length.Percent(100);
        this.style.width = widthVal;
        for (int i = 0; i < 3; i++)
        {
            usageDepths[i] = this.Q("UsableDepths").Q("dot" + (i + 1));
            targetableDepths[i] = this.Q("TargetableDepths").Q("dot" + (i+1));
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
