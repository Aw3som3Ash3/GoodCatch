using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishProfile : VisualElement
{
    VisualElement fishIcon;
    ProgressBar healthBar,xpBar;
    Label nameTitle, levelText;
    // Start is called before the first frame update
    public new class UxmlFactory : UxmlFactory<FishProfile, FishProfile.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public FishProfile()
    {
        Init();
    }
    public FishProfile(FishMonster fishMonster):this()
    {
        SetFish(fishMonster);


    }

    void Init()
    {
        VisualElement root = this;
        root.Clear();
        root.styleSheets.Clear();
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ModProfile");
        nameTitle = this.Q<Label>("Name");
        levelText = this.Q<Label>("Level");
        fishIcon = this.Q("PixelArtAmount");
        healthBar = this.Q<ProgressBar>("HpBar");
        xpBar = this.Q<ProgressBar>("XpBar");
        xpBar.highValue = 1000;

    }

    public void SetFish(FishMonster fishMonster)
    {
        var iconVal = fishIcon.style.backgroundImage.value;
        iconVal.sprite = fishMonster.Icon;
        fishIcon.style.backgroundImage=iconVal;
        levelText.text = $"Lv.{fishMonster.Level.ToString("000")}";
        healthBar.highValue = fishMonster.MaxHealth;
        healthBar.value = fishMonster.Health;
        healthBar.title= $"{fishMonster.Health.ToString("0")}/ {fishMonster.MaxHealth}";
        xpBar.value = fishMonster.Xp;

    }



}
