using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AquariumScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AquariumScreen, AquariumScreen.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public AquariumScreen()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/PauseMenu.uxml");
        visualTreeAsset.CloneTree(root);
    }
}
