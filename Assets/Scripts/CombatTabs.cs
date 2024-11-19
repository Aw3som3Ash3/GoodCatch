using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatTabs : TabbedView
{
    // Start is called before the first frame update
    Button move=new Button();
    AbilityButton[] buttons= new AbilityButton[4];
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
        fightTab.name = "FightTab";
        this.AddTab(fightTab, true);

        VisualElement items = new VisualElement();
        items.name = "Items";
        content.Add(items);
        TabButton itemsTab = new TabButton("Items", items);
        itemsTab.name = "ItemsTab";
        this.AddTab(itemsTab, false);

        VisualElement swap = new VisualElement();
        swap.name = "Swap";
        content.Add(swap);
        TabButton swapTab = new TabButton("Swap", swap);
        swapTab.name = "SwapTab";
        this.AddTab(swapTab, false);
        
        
    }
}
