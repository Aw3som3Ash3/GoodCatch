using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static GameManager;

public class Bestiary : PausePage
{
    ListView fishList;
    Label fishTitle, location, timeOfDay, baits, stamina, hp, agilityMin, attackMin, magicAttackMin, defenseMin, magicDefenseMin, agilityMax, attackMax, magicAttackMax, defenseMax, magicDefenseMax;
    List<FishMonsterType> fishMonsters 
    { get 
        { 

            
            return GameManager.Instance.Database.fishMonsters; 
        } 
    }
    List<bool> hasSeenFish { get { return GameManager.Instance.HasSeenFish; } }

    Label fishLabel;
    VisualElement fishPic;
    //BestiaryPage bestiaryPage;
    int previousSelectedIndex;
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
        //fishList.RegisterCallback<NavigationMoveEvent>(OnNavigate);
        //fishList.itemsChosen += ChoseItem;
        //bestiaryPage = new BestiaryPage();
        this.focusable = true;
        this.delegatesFocus = true;


        fishTitle = this.Q<Label>("NameAmount");
        location = this.Q<Label>("LocationAmount");
        timeOfDay = this.Q<Label>("TimeOfDayAmount");
        baits = this.Q<Label>("BaitAmount");
        stamina = this.Q<Label>("StaAmount");
        hp = this.Q<Label>("HPAmount");

        //agilityMin = this.Q<Label>("MinimumAgilityAmount");
        //attackMin = this.Q<Label>("MinimumPhysicalAttackAmount");
        //magicAttackMin = this.Q<Label>("MinimumMagicalAttackAmount");
        //defenseMin = this.Q<Label>("MinimumPFAmount");
        //magicDefenseMin = this.Q<Label>("MinimumMFAmount");


        //agilityMax = this.Q<Label>("MaximumAgilityAmount");
        //attackMax = this.Q<Label>("MaximumPhysicalAttackAmount");
        //magicAttackMax = this.Q<Label>("MaximumMagicalAttackAmount");
        //defenseMax = this.Q<Label>("MaximumPFAmount");
        //magicDefenseMax = this.Q<Label>("MaximumMFAmount");
        //fishList.Children().First().Focus();
    }

    private void OnNavigate(NavigationMoveEvent evt)
    {
        Debug.Log("nav event");
        if(previousSelectedIndex!= fishList.selectedIndex)
        {
            Debug.Log("should chnage selection too " + previousSelectedIndex);
            fishList.SetSelection(previousSelectedIndex);
            evt.PreventDefault();
        }

       
    }

    //private void ChoseItem(IEnumerable<object> enumerable)
    //{
    //    FishMonsterType fishMonsterType = fishList.selectedItem as FishMonsterType;
    //    if (hasSeenFish[fishMonsterType.fishId] == false)
    //    {
    //        return;
    //    }
    //    this.delegatesFocus = false;
    //    //this.parent.Add(bestiaryPage);
    //    bestiaryPage.SetPage(fishMonsterType);
    //    previousSelectedIndex = fishList.selectedIndex;
    //    fishList.SetEnabled(false);
    //    //fishList.visible=(false);
    //    //this.Q("BookBG").visible=false;
    //}

    private void SelectionChanged(IEnumerable<object> enumerable)
    {

        
        FishMonsterType fishMonsterType = fishList.selectedItem as FishMonsterType;
        if (fishMonsterType ==null) 
        {
            return;
        }
        fishLabel.text = hasSeenFish[fishMonsterType.fishId]? fishMonsterType.name:"????????????";
        var value = fishPic.style.backgroundImage.value;
        value.sprite= hasSeenFish[fishMonsterType.fishId] ? fishMonsterType?.Icon:null;
        fishPic.style.backgroundImage = value;
        SetPage(fishMonsterType);
        //previousSelectedIndex = fishList.selectedIndex;
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
            (item as BestiarySlot).SetFish(fishList.itemsSource[index] as FishMonsterType, hasSeenFish[(fishList.itemsSource[index] as FishMonsterType).fishId]);

        };


        // Set a fixed item height matching the height of the item provided in makeItem. 
        // For dynamic height, see the virtualizationMethod property.
        fishList.fixedItemHeight = 65;


        fishList.itemsSource =fishMonsters.Skip(1).ToList();
    }
    public void SetPage(FishMonsterType fishMonsterType)
    {

        Debug.Log(this.Q<MarinedexTabs>().Q("Stats-content"));

        agilityMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumAgilityAmount");
        attackMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumPhysicalAttackAmount");
        magicAttackMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumMagicalAttackAmount");
        defenseMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumPFAmount");
        magicDefenseMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumMFAmount");


        agilityMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumAgilityAmount");
        attackMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumPhysicalAttackAmount");
        magicAttackMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumMagicalAttackAmount");
        defenseMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumPFAmount");
        magicDefenseMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumMFAmount");

        Debug.Log(fishMonsterType);
        Debug.Log(agilityMin);
        //fishTitle.text = fishMonsterType.name; 
        //stamina.text = fishMonsterType.BaseStamina.ToString();
        //hp.text=fishMonsterType.BaseHealth.ToString();
        if (hasSeenFish[fishMonsterType.fishId])
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
        }
       
    }
    public override bool Back()
    {

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
    Label fishTitle,location,timeOfDay,baits,stamina,hp,agilityMin,attackMin,magicAttackMin,defenseMin,magicDefenseMin, agilityMax, attackMax, magicAttackMax, defenseMax, magicDefenseMax;
    
    
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
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/MarinIndexPage");

        visualTreeAsset.CloneTree(root);
        fishTitle = this.Q<Label>("NameAmount");
        location = this.Q<Label>("LocationAmount");
        timeOfDay = this.Q<Label>("TimeOfDayAmount");
        baits = this.Q<Label>("BaitAmount");
        stamina = this.Q<Label>("StaAmount");
        hp = this.Q<Label>("HPAmount");

       

        this.style.position = Position.Absolute;
        this.StretchToParentSize();
    }
    public void SetPage(FishMonsterType fishMonsterType)
    {

        Debug.Log(this.Q<MarinedexTabs>().Q("Stats-content"));
        agilityMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumAgilityAmount");
        attackMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumPhysicalAttackAmount");
        magicAttackMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumMagicalAttackAmount");
        defenseMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumPFAmount");
        magicDefenseMin = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MinimumMFAmount");


        agilityMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumAgilityAmount");
        attackMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumPhysicalAttackAmount");
        magicAttackMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumMagicalAttackAmount");
        defenseMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumPFAmount");
        magicDefenseMax = this.Q<MarinedexTabs>().Q("unity-content-container").Q("Stats-content").Q<Label>("MaximumMFAmount");


        Debug.Log(fishMonsterType);
        Debug.Log(agilityMin);
        //fishTitle.text = fishMonsterType.name; 
        //stamina.text = fishMonsterType.BaseStamina.ToString();
        //hp.text=fishMonsterType.BaseHealth.ToString();
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
    }

}