using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MarinedexTabs : TabbedMenu
{
    const string statComponentName="ModStatCompontent";
    const string loreComponentName="ModMLore";
    const string abilityComponentName= "ModMAbility";

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
            //dex.statComponentName = m_statComponentName.GetValueFromBag(bag, cc);
            //dex.infoComponentName = m_infoComponentName.GetValueFromBag(bag, cc);
            //dex.localeComponentName = m_localeComponentName.GetValueFromBag(bag, cc);
            
        }
    }

    public MarinedexTabs()
    {
        Init();

    }
    void Init()
    {
        CreateTab("Stats", statComponentName);
        CreateTab("Abilities", abilityComponentName);
        CreateTab("Lore", loreComponentName);
        InputManager.Input.UI.ChangeTab.performed += (x) => ChangeTab((int)x.ReadValue<float>());
        //dex.CreateTab("Info", dex.infoComponentName);
        //dex.CreateTab("Locale", dex.localeComponentName);
    }

    protected override void OnChangedTab(VisualElement element)
    {
        var children = element.Children();
        if (children.First().focusable)
        {
            children.First().Focus();
        }
        else
        {
            children.First().Children().First().Focus();
        }

    }
   
    void CreateTab(string tabName,VisualElement content)
    {
        var tab=new TabMenuButton(tabName, content);
        tab.focusable = false;
        
        Add(tab);
    }
    void CreateTab(string tabName, string contentPath)
    {
        VisualElement content = new VisualElement();
        content.name = tabName+"-content";
      
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/"+contentPath);
        if (visualTreeAsset!=null)
        {
            visualTreeAsset.CloneTree(content);
            if (content != null)
            {
                CreateTab(tabName + "-tab", content);
            }
        }
    }


}
