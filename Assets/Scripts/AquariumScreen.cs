using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AquariumScreen : VisualElement
{
    //VisualElement[] tabs=new VisualElement[3];
    FishventoryTab fishventoryTab;
    
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
        //for (int i = 0; i < tabs.Length; i++)
        //{
        //    tabs[i] = this.Q("box" + (i + 1));
        //    Debug.Log("has box "+i+":  "+ tabs[i]);
        //}
        //contentContainer = this.Q("unity-content-container");
        SetUp();
    }

    void SetUp()
    {
        foreach(var fish in GameManager.Instance.StoredFishventory.Fishies)
        {
            fishventoryTab.tabContent[0].Add(new AquariumSlot(fish));
        }
       
    }
}



public class AquariumSlot : VisualElement
{

    FishMonster fishMonster;
    VisualElement slotBox;
    VisualElement sprite;
    public new class UxmlFactory : UxmlFactory<AquariumSlot, AquariumSlot.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public FishMonster Swap(FishMonster fishMonster)
    {
        var temp = this.fishMonster;
        this.fishMonster = fishMonster;
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
        sprite.style.backgroundImage = fishMonster.MiniSprite;
    }
    void Init()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/AquariumSlot.uxml");
        visualTreeAsset.CloneTree(root);
        slotBox = this.Q("SlotBox");
        sprite = this.Q("Sprite");
    }
}

