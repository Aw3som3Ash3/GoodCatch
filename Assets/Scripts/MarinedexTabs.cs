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
    Label fishTitle, location, timeOfDay, baits, stamina, hp, agilityMin, attackMin, magicAttackMin, defenseMin, magicDefenseMin, agilityMax, attackMax, magicAttackMax, defenseMax, magicDefenseMax, loreAmount, healthAmount, staminaAmount, accuracyMin, accuracyMax, type1, type2;
    VisualElement abilities, stats;
    AbilityButton[] abilityButtons= new AbilityButton[6];
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
        stats=CreateTab("Stats", statComponentName);



        agilityMin = stats.Q<Label>("MinimumAgilityAmount");
        attackMin = stats.Q<Label>("MinimumPhysicalAttackAmount");
        magicAttackMin = stats.Q<Label>("MinimumMagicalAttackAmount");
        defenseMin = stats.Q<Label>("MinimumPFAmount");
        magicDefenseMin = stats.Q<Label>("MinimumMFAmount");


        agilityMax = stats.Q<Label>("MaximumAgilityAmount");
        attackMax = stats.Q<Label>("MaximumPhysicalAttackAmount");
        magicAttackMax = stats.Q<Label>("MaximumMagicalAttackAmount");
        defenseMax = stats.Q<Label>("MaximumPFAmount");
        magicDefenseMax = stats.Q<Label>("MaximumMFAmount");

        healthAmount = stats.Q<Label>("HealthAmount");
        staminaAmount = stats.Q<Label>("StaminaAmount");
        accuracyMin = stats.Q<Label>("MinimumAccuracyAmount");
        accuracyMax = stats.Q<Label>("MaximumAccuracyAmount");

        abilities =CreateTab("Abilities", abilityComponentName);


        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i] = abilities.Q<AbilityButton>("Ability" + i);
        }
        //CreateTab("Lore", loreComponentName);
        InputManager.Input.UI.ChangeTab.performed += (x) => ChangeTab((int)x.ReadValue<float>());
        InputManager.Input.UI.ChangeTab.Enable();

        //dex.CreateTab("Info", dex.infoComponentName);
        //dex.CreateTab("Locale", dex.localeComponentName);
    }
    void SetStatPage(FishMonsterType fishMonsterType)
    {
        if (GameManager.Instance.HasSeenFish[fishMonsterType.fishId])
        {
            agilityMax.text = fishMonsterType.Agility.Max.ToString();
            agilityMin.text = fishMonsterType.Agility.Min.ToString();
            attackMin.text = fishMonsterType.Attack.Min.ToString();
            attackMax.text = fishMonsterType.Attack.Max.ToString();

            magicAttackMax.text = fishMonsterType.Special.Max.ToString();
            magicAttackMin.text = fishMonsterType.Special.Min.ToString();

            magicDefenseMax.text = fishMonsterType.SpecialFortitude.Max.ToString();
            magicDefenseMin.text = fishMonsterType.SpecialFortitude.Min.ToString();
            defenseMin.text = fishMonsterType.Fortitude.Min.ToString();
            defenseMax.text = fishMonsterType.Fortitude.Max.ToString();

            healthAmount.text = fishMonsterType.BaseHealth.ToString();
            staminaAmount.text = fishMonsterType.BaseStamina.ToString();
            accuracyMin.text = fishMonsterType.Accuracy.Min.ToString();
            accuracyMax.text = fishMonsterType.Accuracy.Max.ToString();



        }
        else
        {
            agilityMax.text = "??";
            agilityMin.text = "??";
            attackMin.text = "??";
            attackMax.text = "??";

            magicAttackMax.text = "??";
            magicAttackMin.text = "??";

            magicDefenseMax.text = "??";
            magicDefenseMin.text = "??";
            defenseMin.text = "??";
            defenseMax.text = "??";

            staminaAmount.text = "??";
            healthAmount.text = "??";
            accuracyMin.text = "??";
            accuracyMax.text = "??";
            loreAmount.text = "???";
        }
    }
    void SetAbilityPage(FishMonsterType fishMonsterType)
    {
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            var ability = fishMonsterType.BaseAbilities[i].GetAbility();
            abilityButtons[i].SetAbility(ability,0,0);
        }
    }
    public void SetPage(FishMonsterType fishMonsterType)
    {
        SetStatPage(fishMonsterType);
        SetAbilityPage(fishMonsterType);
        //fishTitle.text = fishMonsterType.name; 
        //stamina.text = fishMonsterType.BaseStamina.ToString();
        //hp.text=fishMonsterType.BaseHealth.ToString();


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
    VisualElement CreateTab(string tabName, string contentPath)
    {
        VisualElement content = new VisualElement();
        content.name = tabName+"-content";
      
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/"+contentPath);
        if (visualTreeAsset!=null)
        {
            visualTreeAsset.CloneTree(content);
            if (content != null)
            {
                CreateTab(tabName, content);
                return content;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
        
    }


}
