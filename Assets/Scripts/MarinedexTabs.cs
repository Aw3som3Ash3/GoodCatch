using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MarinedexTabs : TabbedMenu
{
    string statComponentName,infoComponentName,localeComponentName;

    public new class UxmlFactory : UxmlFactory<MarinedexTabs, UxmlTraits> { }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {
        UxmlStringAttributeDescription m_statComponentName=new UxmlStringAttributeDescription { name = "stat-uxml", defaultValue = "" };
        UxmlStringAttributeDescription m_infoComponentName= new UxmlStringAttributeDescription { name = "info-uxml", defaultValue = "" };
        UxmlStringAttributeDescription m_localeComponentName = new UxmlStringAttributeDescription { name = "locale-uxml", defaultValue = "" };
       
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var dex = (ve as MarinedexTabs);
            dex.statComponentName = m_statComponentName.GetValueFromBag(bag,cc);
            dex.infoComponentName = m_infoComponentName.GetValueFromBag(bag, cc); ;
            dex.localeComponentName = m_localeComponentName.GetValueFromBag(bag, cc); ;
            dex.CreateTab("Stats", dex.statComponentName);
            dex.CreateTab("Info", dex.infoComponentName);
            dex.CreateTab("Locale", dex.localeComponentName);
        }
    }

        public MarinedexTabs()
    {
        Init();
    }
    void Init()
    {
        
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
        if (visualTreeAsset!=null)
        {
            visualTreeAsset.CloneTree(content);
            if (content != null)
            {
                var tab = new TabMenuButton(tabName, content);
                Add(tab);
            }
        }
           
       
       
    }
}
