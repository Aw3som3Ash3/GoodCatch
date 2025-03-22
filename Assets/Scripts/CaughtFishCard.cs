using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CaughtFishCard : VisualElement
{
    VisualElement fishPlacement;
    VisualElement backCard;
    Label health, stamina, attack, defence, magic, resist, accuracy, agility;
    Button flip;
    VisualElement outline;
    bool flipState;
    long animaDuration=500;
    public new class UxmlFactory : UxmlFactory<CaughtFishCard, CaughtFishCard.UxmlTraits>
    {

    }
    public CaughtFishCard()
    {
        Init();
    }
    public CaughtFishCard(FishMonster fishMonster) : this()
    {
        


    }

    void Init()
    {

        VisualElement root = this;
        root.Clear();
        root.styleSheets.Clear();
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ModCardCaught");
        visualTreeAsset.CloneTree(root);


        fishPlacement = this.Q("FishPlacement");
        backCard = this.Q("BackCard");
        health = this.Q<Label>("Health");
        stamina = this.Q<Label>("Stamina");
        attack = this.Q<Label>("Attack");
        defence = this.Q<Label>("defence");
        magic = this.Q<Label>("Magic");
        resist = this.Q<Label>("Resist");
        accuracy = this.Q<Label>("Accucary");
        agility = this.Q<Label>("Agility");

        outline = this.Q("Outline");

        flip = this.Q<Button>("flipflopbutton");
        //animaDuration = (long)(outline.style.transitionDuration.v * 100);

        flip.clicked += FlipCard;


        fishPlacement.SetEnabled(true);
        fishPlacement.visible = true;
        backCard.visible = false;
        backCard.SetEnabled(false);

    }


    

    public void FlipCard()
    {
        outline.style.scale = new Vector2(0, 1);
        schedule.Execute(() =>
        {
            flipState = !flipState;
            fishPlacement.SetEnabled(!flipState);
            fishPlacement.visible = !flipState;
            backCard.visible = flipState;
            backCard.SetEnabled(flipState);
            outline.style.scale = new Vector2(1, 1);


        }).StartingIn(animaDuration);



    }
}
