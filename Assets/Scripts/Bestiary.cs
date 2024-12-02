using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Bestiary : PausePage
{
    ListView fishList;
    List<FishMonsterType> fishMonsters { get { return GameManager.Instance.Database.fishMonsters; } }
    List<bool> hasSeenFish { get { return GameManager.Instance.HasSeenFish; } }

    Label fishLabel;
    VisualElement fishPic;
    BestiaryPage bestiaryPage;
   
    public new class UxmlFactory : UxmlFactory<Bestiary, Bestiary.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public Bestiary()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/BeastBookScroller");

        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        fishList = this.Q<ListView>("FishList");
        fishLabel = this.Q<Label>("HoveredNameAmount");
        fishPic = this.Q("PngAmount");
        this.Q("unity-slider").Children().First().focusable = false;
        SetList();
        fishList.selectionChanged += SelectionChanged;
        fishList.itemsChosen += ChoseItem;
        bestiaryPage = new BestiaryPage();
    }

    private void ChoseItem(IEnumerable<object> enumerable)
    {
        FishMonsterType fishMonsterType = fishList.selectedItem as FishMonsterType;
        if (hasSeenFish[fishMonsterType.fishId] == false)
        {
            return;
        }
        bestiaryPage.SetPage(fishMonsterType);
        this.parent.Add(bestiaryPage);
        this.visible=false;
    }

    private void SelectionChanged(IEnumerable<object> enumerable)
    {

        FishMonsterType fishMonsterType = fishList.selectedItem as FishMonsterType;

        fishLabel.text = hasSeenFish[fishMonsterType.fishId]? fishMonsterType.name:"????????????";
        var value = fishPic.style.backgroundImage.value;
        value.sprite= hasSeenFish[fishMonsterType.fishId] ? fishMonsterType?.Icon:null;
        fishPic.style.backgroundImage = value;

        //throw new NotImplementedException();
    }

    void SetList()
    {
        fishList.makeItem = () =>
        {
            return new BestiarySlot();
        };

        // Set up bind function for a specific list entry
        fishList.bindItem = (item, index) =>
        {

            //(item as Label).text = Path.GetFileNameWithoutExtension(files[index].Name);
            (item as BestiarySlot).SetFish(fishMonsters[index], hasSeenFish[fishMonsters[index].fishId]);

        };


        // Set a fixed item height matching the height of the item provided in makeItem. 
        // For dynamic height, see the virtualizationMethod property.
        fishList.fixedItemHeight = 65;


        fishList.itemsSource =fishMonsters;
    }

    public override bool Back()
    {

        if (this.parent.Contains(bestiaryPage))
        {
            this.parent.Remove(bestiaryPage);
            this.visible = true;

            return false;
        }
        return base.Back();

    }
}


public class BestiarySlot : VisualElement
{
    Label fishId,fishName;
    FishMonsterType fishMonsterType;
    public new class UxmlFactory : UxmlFactory<BestiarySlot, BestiarySlot.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public BestiarySlot()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/BestiaryButton");
        visualTreeAsset.CloneTree(root);
        fishName = this.Q<Label>("Name");
        fishId = this.Q<Label>("EntryNumber");

    }

    public void SetFish(FishMonsterType fishMonsterType,bool wasSeen=true)
    {
        this.fishMonsterType= fishMonsterType;
        fishName.text = wasSeen? fishMonsterType.name:"???????????";
        fishId.text = fishMonsterType.fishId.ToString();
    }
}



public class BestiaryPage:VisualElement
{
    Label fishTitle,location,timeOfDay,baits,stamina,hp,agility,attack,magicAttack,defense,magicDefense;
    
    public new class UxmlFactory : UxmlFactory<BestiaryPage, BestiaryPage.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public BestiaryPage()
    {
        Init();

    }
    void Init()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/BeastiaryContentPage");

        visualTreeAsset.CloneTree(root);
        fishTitle = this.Q<Label>("NameAmount");
        location = this.Q<Label>("LocationAmount");
        timeOfDay = this.Q<Label>("TimeOfDayAmount");
        baits = this.Q<Label>("BaitAmount");
        stamina = this.Q<Label>("StaAmount");
        hp = this.Q<Label>("HPAmount");
        agility = this.Q<Label>("AgiAmount");
        attack = this.Q<Label>("AtkAmount");
        magicAttack = this.Q<Label>("MgAtkAmount");
        defense = this.Q<Label>("FortAmount");
        magicDefense = this.Q<Label>("MgForAmount");
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
    }
    public void SetPage(FishMonsterType fishMonsterType)
    {
        fishTitle.text = fishMonsterType.name;
        stamina.text = fishMonsterType.BaseStamina.ToString();
        hp.text=fishMonsterType.BaseHealth.ToString();
        agility.text=fishMonsterType.Agility.Min+"-"+fishMonsterType.Agility.Max;
        attack.text = fishMonsterType.Attack.Min + "-" + fishMonsterType.Attack.Max;
        magicAttack.text = fishMonsterType.Special.Min + "-" + fishMonsterType.Attack.Max;
        magicDefense.text = fishMonsterType.SpecialFortitude.Min + "-" + fishMonsterType.SpecialFortitude.Max;
        defense.text = fishMonsterType.Fortitude.Min + "-" + fishMonsterType.Fortitude.Max;
    }

}