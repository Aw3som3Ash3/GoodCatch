using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishProfile : VisualElement
{
    VisualElement fishIcon;
    ProgressBar healthBar,xpBar;
    Label nameTitle, levelText;
    FishMonster fishMonster;
    int baseLevel;
    // Start is called before the first frame update
    public new class UxmlFactory : UxmlFactory<FishProfile, FishProfile.UxmlTraits>
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
        visualTreeAsset.CloneTree(root);
        nameTitle = this.Q<Label>("Name");
        levelText = this.Q<Label>("Level");
        fishIcon = this.Q("PixelArtAmount");
        healthBar = this.Q<ProgressBar>("HpBar");
        xpBar = this.Q<ProgressBar>("XpBar");
        xpBar.highValue = 1000;
        

    }

    public void SetFish(FishMonster fishMonster)
    {
        nameTitle.text = fishMonster.Name;
        var iconVal = fishIcon.style.backgroundImage.value;
        iconVal.sprite = fishMonster.MiniSprite;
        fishIcon.style.backgroundImage=iconVal;
        levelText.text = $"Lv.{fishMonster.Level.ToString("000")}";
        healthBar.highValue = fishMonster.MaxHealth;
        healthBar.value = fishMonster.Health;
        healthBar.title= $"{fishMonster.Health.ToString("0")}/ {fishMonster.MaxHealth}";
        xpBar.value = fishMonster.Xp;
        baseLevel = fishMonster.Level;
        this.fishMonster = fishMonster;
    }

    public void UpdateValues()
    {
        levelText.text = $"Lv.{fishMonster.Level.ToString("000")}";
        healthBar.highValue = fishMonster.MaxHealth;
        healthBar.value = fishMonster.Health;
        healthBar.title = $"{fishMonster.Health.ToString("0")}/ {fishMonster.MaxHealth}";
        xpBar.value = fishMonster.Xp;
        Debug.Log(fishMonster.Xp);

    }
    public bool UpdateXpManual(float xpToAdd)
    {
        xpBar.value += xpToAdd;
        Debug.Log(fishMonster.Xp);
        if (xpBar.value > xpBar.highValue)
        {
            xpBar.value %= xpBar.highValue;

            return true;
        }
        else
        {
            return false;
        }

    }
    public void UpdateLevelManual(int level)
    {
        Debug.Log(fishMonster.Xp);
        baseLevel += level;
        levelText.text = $"Lv.{baseLevel.ToString("000")}";

    }



}
