using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewMonsters : VisualElement
{
    VisualElement monsterIcon;
    ProgressBar healthBar, xpBar;
    Label nameTitle, levelText, physical, defence, accuracy, magical, resistance, agility;
    FishMonster fishmonster;


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
        healthBar = this.Q<ProgressBar>("healthBar");
        xpBar = this.Q<ProgressBar>("xpBar");
        physical = this.Q<Label>("physical");
        defence = this.Q<Label>("defence");
        accuracy = this.Q<Label>("accuracy");
        magical = this.Q<Label>("magical");
        resistance = this.Q<Label>("resistance");
        agility = this.Q<Label>("agility");
        xpBar.highValue = 1000;
    }
    public void SetFish(FishMonster fishMonster)
    {
        var iconVal = monsterIcon.style.backgroundImage.value;
        iconVal.sprite = fishMonster.MiniSprite;
        monsterIcon.style.backgroundImage = iconVal;
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
        this.fishmonster = fishMonster;
    }
    
}

