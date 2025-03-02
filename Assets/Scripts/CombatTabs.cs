using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatTabs : TabbedMenu
{
    // Start is called before the first frame update
    Button move=new Button();
    AbilityButton[] buttons= new AbilityButton[3];
    TabMenuButton[] tabs= new TabMenuButton[3];

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
        TabMenuButton fightTab = new TabMenuButton("Fight", abilities);
        fightTab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(fightTab); index = 0; });
        fightTab.name = "FightTab";
        fightTab.focusable = false;
        this.AddTab(fightTab, true);
        tabs[0]= fightTab;

        VisualElement items = new VisualElement();
        items.name = "Items";
        content.Add(items);
        TabMenuButton itemsTab = new TabMenuButton("Items", items);
        itemsTab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(itemsTab); index = 1; });
        itemsTab.name = "ItemsTab";
        itemsTab.focusable = false;
        this.AddTab(itemsTab, false);
        tabs[1]= itemsTab;


        VisualElement swap = new VisualElement();
        swap.name = "Swap";
        content.Add(swap);
        TabMenuButton swapTab = new TabMenuButton("Swap", swap);
        swapTab.RegisterCallback<NavigationSubmitEvent>((x) => { Activate(swapTab); index = 2; });
        swapTab.name = "SwapTab";
        swapTab.focusable = false;
        this.AddTab(swapTab, false);
        tabs[2]= swapTab;
        
        
    }

    public override void ChangeTab(int delta)
    {
        base.ChangeTab(delta);
        FocusFirst();
    }

    public void FocusFirst()
    {
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
    public void FocusOn(int childIndex)
    {
        var children = tabs[index].Target.Children();
        if (children.First().focusable)
        {
            children.ToArray()[childIndex].Focus();
            Debug.Log("refocuses on " + children.ToArray()[childIndex].name);
        }
        else
        {
            children.First().Children().ToArray()[childIndex].Focus();
        }

    }
}
