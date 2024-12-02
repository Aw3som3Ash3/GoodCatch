using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VolumeComponent;

public class FishventoryTab : TabbedView
{
    int numberOfTabs = 3;
    
    public  List<TabButton> tabs=new();
    public List<VisualElement> tabContent = new();
    //int index;
    // Start is called before the first frame update
    public new class UxmlFactory : UxmlFactory<FishventoryTab, FishventoryTab.UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        //UxmlIntAttributeDescription m_NumberOfTabs = new UxmlIntAttributeDescription { name = "NumberOfTabs",defaultValue=3 };
        
        //public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        //{
            
        //    base.Init(ve, bag, cc);

        //    ((FishventoryTab)ve).NumberOfTabs = m_NumberOfTabs.GetValueFromBag(bag, cc);
        //    Debug.Log(((FishventoryTab)ve).NumberOfTabs);
        //}
    }
    public FishventoryTab()
    {
        //m_NumberOfTabs ;
        Init();
    }

    void Init()
    {
        Debug.Log(numberOfTabs);
        var content = this.Q("unity-content-container");
        for(int i = 0; i < numberOfTabs; i++)
        {
           
            VisualElement box = new VisualElement();
            box.name = "Box" + (i + 1);
            box.AddToClassList("Aquarium-Box");
            tabContent.Add(box);
            Debug.Log(box);
            content.Add(box);
            
            TabButton tab = new TabButton("Tab "+(i + 1), box);
            //tab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(tab);});
            tab.name = "Tab"+(i + 1);
            this.AddTab(tab,false);
            tab.tabIndex = 1;
            tabs.Add(tab);
        }
       


    }
}
