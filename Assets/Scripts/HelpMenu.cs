using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HelpMenu : VisualElement
{
    Button fButton, cButton, eButton, aButton, sButton, oButton, hButton, hoButton, tButton;
    ScrollView description;
    VisualElement fText, cText, eText, aText, sText, oText, hText, hoText, tText;

    public HelpMenu()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/HelpHelp");

        visualTreeAsset.CloneTree(root);

        fButton = this.Q<Button>("FishingButton");
        cButton = this.Q<Button>("CombatButton");
        eButton = this.Q<Button>("ElementalButton");
        aButton = this.Q<Button>("AbilityButton");
        sButton = this.Q<Button>("StatusButton");
        oButton = this.Q<Button>("OtherButton");
        hButton = this.Q<Button>("HealingButton");
        hoButton = this.Q<Button>("HooksButton");
        tButton = this.Q<Button>("TravelingButton");

        description = this.Q<ScrollView>("Description");

        LoadText(ref fText, "UXMls/ModHelpFishing");
        LoadText(ref cText, "UXMls/ModHelpCombatTips");
        LoadText(ref eText, "UXMls/ModHelpElementalTypes");
        LoadText(ref aText, "UXMls/ModHelpAbility");
        LoadText(ref sText, "UXMls/ModHelpStatusEffect");
        LoadText(ref oText, "UXMls/ModHelpOtherEffects");
        LoadText(ref hText, "UXMls/ModHelpHealingItemsInn");
        LoadText(ref hoText, "UXMls/ModHelpCatchingTips");
        LoadText(ref tText, "UXMls/ModHelpTraveling");

        fButton.clicked += () => OnClick(fText);
        cButton.clicked += () => OnClick(cText);
        eButton.clicked += () => OnClick(eText);
        aButton.clicked += () => OnClick(aText);
        sButton.clicked += () => OnClick(sText);
        oButton.clicked += () => OnClick(oText);
        hButton.clicked += () => OnClick(hText);
        hoButton.clicked += () => OnClick(hoText);
        tButton.clicked += () => OnClick(tText);
    }

    void LoadText(ref VisualElement text, string path)
    {
        text = new();
        VisualTreeAsset visualTreeAsset = Resources.Load< VisualTreeAsset>(path);

        visualTreeAsset.CloneTree(text);
    }
    void OnClick(VisualElement element)
    {
        Debug.Log("Element trying to open" + element);
        description.Clear();
        description.Add(element);
    }
}
