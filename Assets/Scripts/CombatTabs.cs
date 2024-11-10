using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatTabs : TabbedView
{
    // Start is called before the first frame update
    Button move=new Button();
    Button[] buttons= new Button[4];
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
            buttons[i]=new Button();
            buttons[i].name = "ability"+i;
            buttons[i].text = "ability"+i;
            buttons[i].AddToClassList("Actions");
            //buttons[i].bindingPath = "ActionButtons";
            abilities.Add(buttons[i]);
        }
        
        content.Add(abilities);
        
        this.AddTab(new TabButton("Fight", abilities), true);

        VisualElement items = new VisualElement();
        items.name = "Items";
        content.Add(items);
        this.AddTab(new TabButton("Items", this.Q("Items")), false);

        VisualElement swap = new VisualElement();
        swap.name = "Swap";
        content.Add(swap);
        this.AddTab(new TabButton("Swap", this.Q("Swap")), false);
        
        
    }
}
