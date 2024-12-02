using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolTipBox : VisualElement
{
    public VisualElement content { get; private set; }

    VisualElement currentTarget;
    
    public new class UxmlFactory : UxmlFactory<ToolTipBox, ToolTipBox.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public ToolTipBox()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/ToolTip.uxml");
        visualTreeAsset.CloneTree(root);
        content = this.Q("content");
        this.visible = false;
        this.style.position = Position.Absolute;
        
    }

    public bool EnableToolTip(VisualElement target)
    {
        this.visible = true;
        if (currentTarget == target)
        {
            return false;
        }
        currentTarget = target;
        var pos = target.style.transformOrigin.value.x;
        this.transform.position = this.parent.WorldToLocal(target.LocalToWorld(Vector2.zero)) + (target.contentRect.xMax * 0.5f) * Vector2.right;
        this.BringToFront();
        return true;
        
    }
}
