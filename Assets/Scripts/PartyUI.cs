using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PartyUI : VisualElement
{
    Fishventory fishventory;
    public GameObject partyPanel;

    VisualElement preview;
    Label description;

    //Dictionary<VisualElement, FishMonster> slotsFish;
    //protected override InputAction input => InputManager.Input.UI.Party;

    public new class UxmlFactory : UxmlFactory<PartyUI, PartyUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public PartyUI()
    {

        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/PartyUI.uxml");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        
        SetUpUI();
        UpdateUI();
    }
    void SetUpUI()
    {
        for (int i = 0; i < 7; i++)
        {
            var element = this.Q("PartyMember" + (i + 1));
            int index = i;
            element.focusable = true;
            element.SetEnabled(true);
            element.RegisterCallback<ClickEvent>((x) => element.Focus());
            element.RegisterCallback<FocusInEvent>((x) => OnFocus(fishventory.Fishies[index]));
            element.RegisterCallback<FocusOutEvent>((x) => OnFocusExit());

        }

        preview = this.Q("Preview");
        description = this.Q<Label>("Description");
    }
    public void UpdateUI()
    {
        fishventory = GameManager.Instance.PlayerFishventory;
        //slotsFish = new();
        for (int i = 0; i < 7; i++)
        {
            var element = this.Q("PartyMember" + (i + 1));
            if (i < fishventory.Fishies.Count)
            {
                int index = i;
                element.Q("PartyIcon").style.backgroundImage = fishventory.Fishies[i].Icon;
                element.SetEnabled(true);
            }
            else
            {
                element.style.backgroundImage = null;
                element.SetEnabled(false);
            }


        }
    }


    void OnFocus(FishMonster fishMonster)
    {

        preview.style.backgroundImage = fishMonster.Icon;
        description.text = fishMonster.description;


    }

    void OnFocusExit()
    {

    }


}