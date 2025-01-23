using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MarinedexTabs : TabbedMenu
{
    public new class UxmlFactory : UxmlFactory<MarinedexTabs, UxmlTraits> { }
    public MarinedexTabs()
    {
        Init();
    }
    void Init()
    {
        CreateTab("Stats", "ModStatCompontent");
        CreateTab("Info", new VisualElement());
        CreateTab("Locale", new VisualElement());
    }
    void CreateTab(string tabName,VisualElement content)
    {
        var tab=new TabMenuButton(tabName, content);
        Add(tab);
    }
    void CreateTab(string tabName, string contentPath)
    {
        VisualElement content = new VisualElement();
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/"+contentPath);

        visualTreeAsset.CloneTree(content);
        var tab = new TabMenuButton(tabName, content);
        Add(tab);
    }
}
