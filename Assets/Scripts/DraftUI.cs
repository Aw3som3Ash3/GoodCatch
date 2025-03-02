using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DraftUI : VisualElement
{
    readonly GoodCatchInputs inputs = InputManager.Input;
    AbilityButton[] abilityButtons = new AbilityButton[3];
    ProgressBar healthBar, staminaBar;
    Label infoScreenHealth, infoScreenStamina, infoScreenAttack, infoScreenMagicAttack, infoScreenDefense, infoScreenMagicDefense, infoScreenAgility;

    public new class UxmlFactory : UxmlFactory<DraftUI, DraftUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public DraftUI()
    {
        Initial();
    }
    public void Initial()
    {

        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/CombatUI 2");

        visualTreeAsset.CloneTree(root);
        this.style.flexGrow = 1;

        inputs.Combat.Enable();
        this.StretchToParentSize();
        this.pickingMode = PickingMode.Ignore;


        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i] = this.Q<AbilityButton>("ability" + i);
            int index = i;
            
        }




        infoScreenAgility = this.Q<Label>("AgilityAmount");
        infoScreenAttack = this.Q<Label>("AtkAmount");
        infoScreenDefense = this.Q<Label>("DefAmount");
        infoScreenMagicAttack = this.Q<Label>("MgAtkAmount");
        infoScreenMagicDefense = this.Q<Label>("MgDefAmount");
        infoScreenHealth = this.Q<Label>("HealthAmount");
        infoScreenStamina = this.Q<Label>("StamAmount");
    }



    void UpdateInfo(FishMonster fish)
    {
        infoScreenAgility.text = fish.Agility.value.ToString();
        infoScreenAttack.text = fish.Attack.value.ToString();
        infoScreenDefense.text = fish.Fortitude.value.ToString();
        infoScreenMagicAttack.text = fish.Special.value.ToString();
        infoScreenMagicDefense.text = fish.SpecialFort.value.ToString();
        infoScreenHealth.text = fish.Health.ToString("00") + "/" + fish.MaxHealth.ToString("00");
        infoScreenStamina.text = fish.MaxStamina.ToString("00");
        if (fish.Type.Elements.Length >= 1)
        {
            this.Q<Label>("Type1Amount").text = fish.Type.Elements[0].name;

        }
        else
        {
            this.Q<Label>("Type1Amount").text = "";
        }
        if (fish.Type.Elements.Length >= 2)
        {
            this.Q<Label>("Type2Amount").text = fish.Type.Elements[1].name;
        }
        else
        {
            this.Q<Label>("Type2Amount").text = "";
        }

    }
}
