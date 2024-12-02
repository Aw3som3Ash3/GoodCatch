using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AquariumScreen : PausePage
{
    //VisualElement[] tabs=new VisualElement[3];
    FishventoryTab fishventoryTab;
    AquariumSlot selectedSlot;
    Label nameTitle,hp,stamina;
    VisualElement picturePreview;
    VisualElement party;
    Button[] partySlots=new Button[7];
    Dictionary<VisualElement, FishMonster> slotToFish=new();
    VisualElement mainOptions;
    Button addToParty;
    public new class UxmlFactory : UxmlFactory<AquariumScreen, AquariumScreen.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public AquariumScreen()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/PcContainer.uxml");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        InputManager.DisablePlayer();
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        fishventoryTab = this.Q<FishventoryTab>();
        nameTitle = this.Q<Label>("NameAmount");
        hp = this.Q<Label>("HpAmount");
        stamina = this.Q<Label>("StaminaAmount");
        picturePreview = this.Q("Png");
        party = this.Q("CombatDraftUI");
        mainOptions = this.Q("MostLeftContainerPC");
        mainOptions.visible = true;
        mainOptions.SetEnabled(true);
        for (int i = 0; i < 7; i++)
        {
            int index = i;
            partySlots[i]=party.Q<Button>("slot" + (i + 1));
            if (i < GameManager.Instance.PlayerFishventory.Fishies.Count)
            {
                var value = partySlots[i].style.backgroundImage.value;
                value.sprite = GameManager.Instance.PlayerFishventory.Fishies[i].Icon;
                slotToFish[partySlots[i]] = GameManager.Instance.PlayerFishventory.Fishies[i];
            }
           
           

            partySlots[i].clicked += () =>
            {
                var temp = selectedSlot.Swap(slotToFish[partySlots[index]]);
                GameManager.Instance.PlayerFishventory.SwapFish(index, temp);
                var value = partySlots[index].style.backgroundImage.value;
                value.sprite= GameManager.Instance.PlayerFishventory.Fishies[index].Icon;
                slotToFish[partySlots[index]] = GameManager.Instance.PlayerFishventory.Fishies[index];
                mainOptions.visible = true;
                mainOptions.SetEnabled(true);
                party.visible = false;
                party.SetEnabled(false);
                OnSelect(selectedSlot);
            };
        }
        party.visible = false;
        party.SetEnabled(false);
        addToParty = this.Q<Button>("AddToPartyButton");
        addToParty.clicked += OnAddToParty;
        //for (int i = 0; i < tabs.Length; i++)
        //{
        //    tabs[i] = this.Q("box" + (i + 1));
        //    Debug.Log("has box "+i+":  "+ tabs[i]);
        //}
        //contentContainer = this.Q("unity-content-container");
        SetUp();
    }

    private void OnAddToParty()
    {

        if (selectedSlot == null)
        {
            return;
        }
        party.visible = true;
        party.SetEnabled(true);
        mainOptions.visible = false;
        mainOptions.SetEnabled(false);
        //for (int i = 0; i < partySlots.Length; i++)
        //{
        //    int index = i;
                      
        //}

        //throw new NotImplementedException();
    }

    void SetUp()
    {
        foreach(var fish in GameManager.Instance.StoredFishventory.Fishies)
        {
            var slot = new AquariumSlot(fish);
            fishventoryTab.tabContent[0].Add(slot);
            slot.Selected += OnSelect;
        }
       
    }
    void OnSelect(AquariumSlot slot)
    {

        selectedSlot?.UnSelect();
        selectedSlot = slot;
        if (slot.fishMonster == null)
        {
            return;
        }
        var value = picturePreview.style.backgroundImage.value;
        value.sprite= slot.fishMonster.Icon;
        nameTitle.text = slot.fishMonster.Name;
        hp.text = slot.fishMonster.MaxHealth.ToString();
        stamina.text = slot.fishMonster.MaxStamina.ToString();
    }
}



public class AquariumSlot : VisualElement
{

    public FishMonster fishMonster { get; private set; }
    VisualElement slotBox;
    VisualElement sprite;
    const string AQUARIUM_SLOT_SELECTED_CLASS = "Aquarium-slot__Selected";
    const string AQUARIUM_SLOT_CLASS = "Aquarium-slot";
    public event Action<AquariumSlot> Selected;
    public new class UxmlFactory : UxmlFactory<AquariumSlot, AquariumSlot.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public void Select()
    {
        this.AddToClassList(AQUARIUM_SLOT_SELECTED_CLASS);
        
        Selected?.Invoke(this);
    }
    public void UnSelect()
    {
        this.RemoveFromClassList(AQUARIUM_SLOT_SELECTED_CLASS);
    }
    public FishMonster Swap(FishMonster fishMonster)
    {
        var temp = this.fishMonster;
        this.fishMonster = fishMonster;
        var value= sprite.style.backgroundImage.value;
        value.sprite= fishMonster.MiniSprite;
        return temp;
    }
    public AquariumSlot()
    {
        Init();
        sprite.visible = false;
    }
    public AquariumSlot(FishMonster fishMonster)
    {
        Init();
        this.fishMonster = fishMonster;
        var value = sprite.style.backgroundImage.value;
        value.sprite= fishMonster.MiniSprite;
    }
    void Init()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/AquariumSlot.uxml");
        visualTreeAsset.CloneTree(root);
        slotBox = this.Q("SlotBox");
        sprite = this.Q("Sprite");
        this.focusable = true;
        this.AddToClassList(AQUARIUM_SLOT_CLASS);
        this.AddManipulator(new Clickable(Select));
        //this.RegisterCallback<MouseOverEvent>((x) => this.BringToFront());
        //this.RegisterCallback<MouseexitEvent>((x) => this.BringToFront());
        this.style.flexGrow = 0;
        
        

    }
}

