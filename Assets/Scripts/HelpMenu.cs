using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HelpMenu : VisualElement
{
    Button fishingButton, combatButton, elementalButton, abilityButton, statusButton, otherButton, healingButton, hooksButton, travelingButton;
    ScrollView description;
    VisualElement fishingText, combatText, elementalText, abilityText, statusText, otherText, healingText, hooksText, travelingText;

    public HelpMenu()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/HelpHelp");

        visualTreeAsset.CloneTree(root);

        fishingButton = this.Q<Button>("FishingButton");
        combatButton = this.Q<Button>("CombatButton");
        elementalButton = this.Q<Button>("ElementalButton");
        abilityButton = this.Q<Button>("AbilityButton");
        statusButton = this.Q<Button>("StatusButton");
        otherButton = this.Q<Button>("OtherButton");
        healingButton = this.Q<Button>("HealingButton");
        hooksButton = this.Q<Button>("HooksButton");
        travelingButton = this.Q<Button>("TravelingButton");

        description = this.Q<ScrollView>("Description");

        LoadText(ref fishingText, "UXMls/ModHelpFishing");
        LoadText(ref combatText, "UXMls/ModHelpCombatTips");
        LoadText(ref elementalText, "UXMls/ModHelpElementalTypes");
        LoadText(ref abilityText, "UXMls/ModHelpAbility");
        LoadText(ref statusText, "UXMls/ModHelpStatusEffect");
        LoadText(ref otherText, "UXMls/ModHelpOtherEffects");
        LoadText(ref healingText, "UXMls/ModHelpHealingItemsInn");
        LoadText(ref hooksText, "UXMls/ModHelpCatchingTips");
        LoadText(ref travelingText, "UXMls/ModHelpTraveling");

        fishingButton.clicked += () => OnClick(fishingText);
        combatButton.clicked += () => OnClick(combatText);
        elementalButton.clicked += () => OnClick(elementalText);
        abilityButton.clicked += () => OnClick(abilityText);
        statusButton.clicked += () => OnClick(statusText);
        otherButton.clicked += () => OnClick(otherText);
        healingButton.clicked += () => OnClick(healingText);
        hooksButton.clicked += () => OnClick(hooksText);
        travelingButton.clicked += () => OnClick(travelingText);
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
