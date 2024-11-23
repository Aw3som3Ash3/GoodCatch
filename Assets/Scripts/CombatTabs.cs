using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatTabs : TabbedView
{
    // Start is called before the first frame update
    Button move=new Button();
    AbilityButton[] buttons= new AbilityButton[4];
    TabButton[] tabs= new TabButton[3];

    int index;
    public new class UxmlFactory : UxmlFactory<CombatTabs, UxmlTraits> { }
    public CombatTabs(): base()
    {
        
        var content = this.Q("unity-content-container");
        VisualElement abilities=new VisualElement();
        abilities.name = "Abilities";
        move.name="Move";
        move.text = "Move";
        abilities.Add(move);
        move.AddToClassList("Actions");
        //move.bindingPath = "ActionButtons";
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = new AbilityButton();
            buttons[i] = button;
            buttons[i].name = "ability"+i;
            //buttons[i]. = "ability"+i;
            buttons[i].AddToClassList("Actions");
            //buttons[i].bindingPath = "ActionButtons";
            abilities.Add(button);
        }
        
        content.Add(abilities);
        TabButton fightTab = new TabButton("Fight", abilities);
        fightTab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(fightTab); index = 0; });
        fightTab.name = "FightTab";
        this.AddTab(fightTab, true);
        tabs[0]= fightTab;

        VisualElement items = new VisualElement();
        items.name = "Items";
        content.Add(items);
        TabButton itemsTab = new TabButton("Items", items);
        itemsTab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(itemsTab); index = 1; });
        itemsTab.name = "ItemsTab";
        this.AddTab(itemsTab, false);
        tabs[1]= itemsTab;


        VisualElement swap = new VisualElement();
        swap.name = "Swap";
        content.Add(swap);
        TabButton swapTab = new TabButton("Swap", swap);
        swapTab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(swapTab); index = 2; });
        swapTab.name = "SwapTab";
        this.AddTab(swapTab, false);
        tabs[2]= swapTab;
        
        
    }

    public void ChangeTab(int delta)
    {
        int targetIndex= Mathf.Clamp(index+delta, 0, 2);
        if (index == targetIndex)
        {
            return;
        }
        index = targetIndex;
        Activate(tabs[index]);

        if(tabs[index].Target.childCount<=0)
        {
            return;
        }
        var children = tabs[index].Target.Children();
        if (children.First().focusable)
        {
            children.First().Focus();
        }
        else
        {
            children.First().Children().First().Focus();
        }
       
    }
}
