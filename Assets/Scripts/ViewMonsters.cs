using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewMonsters : VisualElement
{
    VisualElement monsterIcon, elementalIcon1, elementalIcon2;
    ProgressBar healthBar, xpBar;
    Label speciesName,nameTitle, levelText, physical, defence, accuracy, magical, resistance, agility, stamina;
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
    }
    
}

