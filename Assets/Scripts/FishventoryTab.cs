using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishventoryTab : TabbedView
{
    int m_NumberOfTabs;
    public int NumberOfTabs { get; set; }
    public TabButton[] tabs;
    int index;
    // Start is called before the first frame update
    public new class UxmlFactory : UxmlFactory<FishventoryTab, FishventoryTab.UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription m_NumberOfTabs = new UxmlIntAttributeDescription { name = "NumberOfTabs",defaultValue=3 };
        
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            
            base.Init(ve, bag, cc);
            ((FishventoryTab)ve).NumberOfTabs = m_NumberOfTabs.GetValueFromBag(bag, cc);
            Debug.Log(((FishventoryTab)ve).NumberOfTabs);
        }
    }
    public FishventoryTab()
    {
        m_NumberOfTabs = 3;
        Init();
    }

    void Init()
    {
        Debug.Log(m_NumberOfTabs);
        var content = this.Q("unity-content-container");
        tabs =new TabButton[m_NumberOfTabs];
        for(int i = 0; i < m_NumberOfTabs; i++)
        {
            //int index = i;
            VisualElement box = new VisualElement();
            content.Add(box);
            TabButton tab = new TabButton("Tab "+(i+1), box);
            tab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(tab); index = 0; });
            tab.name = "FightTab";
            this.AddTab(tab, true);
            tabs[i] = tab;
        }


    }
}
