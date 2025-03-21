using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpScreen : VisualElement
{
    Label hpOriginal, hpNew,hpDelta;
    Label staminaOriginal, staminaNew,staminaDelta;
    Label attackOriginal, attackNew,attackDelta;
    Label defenceOriginal, defenceNew,defenceDelta;
    Label magicOriginal, magicNew,magicDelta;
    Label resistanceOriginal, resistanceNew,resistanceDelta;
    Label accuracyOriginal, accuracyNew,accuracyDelta;
    Label agilityOriginal, agilityNew,agilityDelta;



    public new class UxmlFactory : UxmlFactory<LevelUpScreen, LevelUpScreen.UxmlTraits>
    {

    }
    public LevelUpScreen()
    {
        Init();
    }

    void Init()
    {
        VisualElement root = this;
        root.Clear();
        root.styleSheets.Clear();
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ModLevelUp");
        visualTreeAsset.CloneTree(root);

        hpNew = this.Q("Health").Q<Label>("OriginalStat");
        hpOriginal = this.Q("Health").Q<Label>("SumStat");
        hpDelta = this.Q("Health").Q<Label>("IncreasedAmount");

        staminaOriginal = this.Q("Stamina").Q<Label>("OriginalStat");
        staminaNew = this.Q("Stamina").Q<Label>("SumStat");
        staminaDelta = this.Q("Stamina").Q<Label>("IncreasedAmount");

        attackOriginal = this.Q("Attack").Q<Label>("OriginalStat");
        attackNew = this.Q("Attack").Q<Label>("SumStat");
        attackDelta = this.Q("Attack").Q<Label>("IncreasedAmount");

        defenceOriginal = this.Q("Defence").Q<Label>("OriginalStat");
        defenceNew = this.Q("Defence").Q<Label>("SumStat");
        defenceDelta = this.Q("Defence").Q<Label>("IncreasedAmount");

        magicOriginal = this.Q("Magic").Q<Label>("OriginalStat");
        magicNew = this.Q("Magic").Q<Label>("SumStat");
        magicDelta = this.Q("Magic").Q<Label>("IncreasedAmount");

        resistanceOriginal = this.Q("Resistance").Q<Label>("OriginalStat");
        resistanceNew = this.Q("Resistance").Q<Label>("SumStat");
        resistanceDelta = this.Q("Resistance").Q<Label>("IncreasedAmount");

        accuracyOriginal = this.Q("Accuracy").Q<Label>("OriginalStat");
        accuracyNew = this.Q("Accuracy").Q<Label>("SumStat");
        accuracyDelta = this.Q("Accuracy").Q<Label>("IncreasedAmount");

        agilityOriginal = this.Q("Agility").Q<Label>("OriginalStat");
        agilityNew = this.Q("Agility").Q<Label>("SumStat");
        agilityDelta = this.Q("Agility").Q<Label>("IncreasedAmount");


    }


    public void SetFish(FishMonster fishMonster,int deltaLevel)
    {
        //fishMonster.Accuracy.talent



    }

    


}
