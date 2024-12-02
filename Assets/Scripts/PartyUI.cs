using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PartyUI : PausePage
{
    Fishventory fishventory;
    public GameObject partyPanel;

    VisualElement preview;
    Label description;
    Label hpAmount, attackAmount, defenseAmount, agilityAmount, staminaAmount, mgAttackAmount, mgDefenseAmount, accuracyAmount;

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
        hpAmount = this.Q<Label>("HpAmount");
        attackAmount = this.Q<Label>("AttackAmount");
        defenseAmount = this.Q<Label>("DefenseAmount");
        agilityAmount = this.Q<Label>("AgilityAmount");
        staminaAmount = this.Q<Label>("StaminaAmount");
        mgAttackAmount = this.Q<Label>("MgAttackAmount");
        mgDefenseAmount = this.Q<Label>("MgDefenseAmount");
        accuracyAmount = this.Q<Label>("AccuracyAmount");
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
                var fish = fishventory.Fishies[i];
                Debug.Log(fish);
                Debug.Log(element);
                element.Q("PartyIcon").style.backgroundImage =fish?.Icon;
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
        hpAmount.text = fishMonster.MaxHealth.ToString(); 
        attackAmount.text = fishMonster.Attack.value.ToString();
        defenseAmount.text = fishMonster.Fortitude.value.ToString();
        agilityAmount.text = fishMonster.Agility.value.ToString();
        staminaAmount.text = fishMonster.MaxStamina.ToString(); 
        mgAttackAmount.text = fishMonster.Special.value.ToString();
        mgDefenseAmount.text = fishMonster.SpecialFort.value.ToString();
        accuracyAmount.text = fishMonster.Accuracy.value.ToString(); 

    }

    void OnFocusExit()
    {

    }


}