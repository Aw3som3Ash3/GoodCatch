using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ViewMonsters : VisualElement
{
    int abilityTabIndex;
    InputAction tabAbility;
    VisualElement monsterIcon, elementalIcon1, elementalIcon2, abilityEffectIcons, abilityElementIcon;
    ProgressBar healthBar, xpBar;
    Label speciesName,nameTitle, levelText, physical, defence, accuracy, magical, resistance, agility, stamina;
    Label abilityScaledDamage, abilityStaminaCost, abilityAccuracy, abilityTargetTeam, abilityUsableDepth,abilityTargetableDepth, abilityName, abilityElement, abilityType, abilityPiercing, abilityMovement;
    FishMonster fishMonster;
    


    public new class UxmlFactory : UxmlFactory<ViewMonsters, UxmlTraits>
    {

    }

    public ViewMonsters()
    {
        Init();
    }

    public ViewMonsters(FishMonster fishMonster) : this()
    {
        SetFish(fishMonster);
    }
    void Init() 
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ViewMonster 1");
        visualTreeAsset.CloneTree(root);
        nameTitle = this.Q<Label>("nameTitle");
        levelText = this.Q<Label>("levelText");
        monsterIcon = this.Q("monsterIcon");
        healthBar = this.Q<ProgressBar>("HpBar");
        xpBar = this.Q<ProgressBar>("xpBar");
        physical = this.Q<Label>("physical");
        defence = this.Q<Label>("defence");
        accuracy = this.Q<Label>("accuracy");
        magical = this.Q<Label>("magical");
        resistance = this.Q<Label>("resistance");
        agility = this.Q<Label>("agility");
        stamina = this.Q<Label>("stamina");
        elementalIcon1 = this.Q("elementalIcon1");
        elementalIcon2 = this.Q("elementalIcon2");
        speciesName = this.Q<Label>("speciesName");
        xpBar.highValue = 1000;
        tabAbility = InputManager.Input.UI.ChangeTab;
        tabAbility.performed += OnAbilityTab;

        abilityScaledDamage = this.Q<Label>("DamageAmount");
        abilityStaminaCost = this.Q<Label>("StaminaAmount");
        abilityAccuracy = this.Q<Label>("AccuracyAmount");
        abilityTargetTeam = this.Q <Label> ("TargetWhomAmount");
        abilityName = this.Q<Label>("AbilityName");
        abilityElement = this.Q<Label>("AbilityElement"); 
        abilityType = this.Q<Label>("AbilityType");
        abilityPiercing = this.Q<Label>("PiercingAmount");
        abilityMovement = this.Q<Label>("MovementAmount");
        abilityElementIcon = this.Q("AbilityElementIcon");
        //abilityUsableDepth = this.Q<Label>("UseLaneAmount");
        //abilityTargetableDepth = this.Q<Label>("TargetLaneAmount");
        abilityEffectIcons = this.Q("PopulateEffects");

   
    }

    private void OnAbilityTab(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<float>();
        if(input < 0)
        {
            abilityTabIndex--;
            if (abilityTabIndex < 0)
            {
                abilityTabIndex = 2;
            }
        }
        else if(input > 0) 
        {
            abilityTabIndex++;
            abilityTabIndex %= 3;
        }
        UpdateAbility();
    }

    public void SetFish(FishMonster fishMonster)
    {
        nameTitle.text = fishMonster.Name;
        speciesName.text = fishMonster.Type.name;
        var iconVal = monsterIcon.style.backgroundImage.value;
        iconVal.sprite = fishMonster.Icon;
        monsterIcon.style.backgroundImage = iconVal;

        if (fishMonster.Type.Elements.Length>=1)
        {
            var elemental = elementalIcon1.style.backgroundImage.value;
            elemental.sprite = fishMonster.Type.Elements[0].Icon;
            elementalIcon1.style.backgroundImage = elemental;
        }
        else
        {
            elementalIcon1.style.backgroundImage = null;
        }

        if (fishMonster.Type.Elements.Length>=2)
        {
            var elemental = elementalIcon2.style.backgroundImage.value;
            elemental.sprite = fishMonster.Type.Elements[1].Icon;
            elementalIcon2.style.backgroundImage = elemental;
        }
        else
        {
            elementalIcon2.style.backgroundImage = null;
        }


        levelText.text = $"Lv.{fishMonster.Level.ToString("000")}";
        healthBar.highValue = fishMonster.MaxHealth;
        healthBar.value = fishMonster.Health;
        healthBar.title = $"{fishMonster.Health.ToString("0")}/ {fishMonster.MaxHealth}";
        xpBar.value = fishMonster.Xp;
        physical.text = fishMonster.Attack.value.ToString();
        defence.text = fishMonster.Fortitude.value.ToString();
        accuracy.text = fishMonster.Accuracy.value.ToString();
        magical.text = fishMonster.Special.value.ToString();
        resistance.text = fishMonster.SpecialFort.value.ToString();
        agility.text = fishMonster.Agility.value.ToString();
        stamina.text = fishMonster.MaxStamina.ToString();
        this.fishMonster = fishMonster;

        UpdateAbility();
    }
    void UpdateAbility()
    {
        Ability ability = fishMonster.GetAbility(abilityTabIndex);
        abilityScaledDamage.text = ability.GetDamage(fishMonster).ToString("00");
        abilityStaminaCost.text = ability.StaminaUsage.ToString();
        abilityAccuracy.text = ability.StaminaUsage.ToString();
        abilityTargetTeam.text = ability.TargetedTeam.ToString();
        abilityName.text = ability.name.ToString();
        if (ability.AbilityPhysicalMagical == Ability.AbilityType.attack)
        {
            abilityType.text = "Physical";
        }
        else
        {
            abilityType.text = "Magical";
        }
        abilityElement.text = ability.Element.name.ToString();
        abilityPiercing.text = ability.Piercing.ToString();
        abilityMovement.text = ability.ForcedMovement.ToString();
        //abilityUsableDepth.text = ability.AvailableDepths.ToString();
        //abilityTargetableDepth.text = ability.TargetableDepths.ToString();
        //The two above Broke

        abilityElementIcon.Clear();
        var value = abilityElementIcon.style.backgroundImage.value;
        value.sprite = ability.Element.Icon;
        abilityElementIcon.style.backgroundImage = value;




        abilityEffectIcons.Clear();
        foreach (var effect in ability.Effects)
        {
            Label label = new();
            label.style.width = 60;
            label.style.height = 60;
            var verdict = label.style.backgroundImage.value;
            verdict.texture = effect.Effect.Icon;
            label.style.backgroundImage = verdict;
            abilityEffectIcons.Add(label);
        }
    }
    
}

