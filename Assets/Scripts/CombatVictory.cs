using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatVictory : VisualElement
{

    public Dictionary<FishMonster, ProgressBar> fishXpBar { get; private set; } = new();
    public new class UxmlFactory : UxmlFactory<CombatVictory, CombatVictory.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public CombatVictory()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/CombatVictory");

        visualTreeAsset.CloneTree(root);
    }

    public CombatVictory(List<FishMonster> playerFishes,List<FishMonster>enemyFishes,Dictionary<FishMonster,bool> fishesCaught)
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/CombatVictory");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        //setting up the player fishes side
        for(int i = 0; i < 7; i++)
        {
            var fishSlot = this.Q("Fish" + (i + 1));
            if (i < playerFishes.Count)
            {
                fishSlot.Q("PlayerContent").visible = true;
                fishSlot.Q("PlayerContent").SetEnabled(true);

                fishSlot.Q("EnemyContent").visible = false;
                fishSlot.Q("EnemyContent").SetEnabled(false);


                fishSlot.Q<ProgressBar>("healthBar").value = playerFishes[i].Health;
                fishSlot.Q<ProgressBar>("healthBar").highValue = playerFishes[i].MaxHealth;
                fishSlot.Q<ProgressBar>("xpBar").value = playerFishes[i].Xp;
                fishXpBar[playerFishes[i]] = fishSlot.Q<ProgressBar>("xpBar");
                var value = fishSlot.Q("ProfilePic").style.backgroundImage.value;
                value.sprite = playerFishes[i].Icon;
                fishSlot.Q("ProfilePic").style.backgroundImage=value;
            }
            else
            {
                fishSlot.visible = false;
                fishSlot.SetEnabled(false);
            }
            var enemySlot = this.Q("enemy" + (i + 1));
            if (i < enemyFishes.Count)
            {
                enemySlot.Q("PlayerContent").visible = false;
                enemySlot.Q("PlayerContent").SetEnabled(false);

                enemySlot.Q("EnemyContent").visible = true;
                enemySlot.Q("EnemyContent").SetEnabled(true);

                Label caughtLabel = enemySlot.Q<Label>("HasBeenCaught");
                caughtLabel.text = fishesCaught[enemyFishes[i]] ? "Caught" : "Released";
                caughtLabel.style.color = fishesCaught[enemyFishes[i]] ?  Color.green: Color.red;

               enemySlot.Q<ProgressBar>("healthBar").visible = false;
                enemySlot.Q<ProgressBar>("xpBar").visible = false;
                var value=enemySlot.Q("ProfilePic").style.backgroundImage.value;
                value.sprite= playerFishes[i].Icon;
                enemySlot.Q("ProfilePic").style.backgroundImage = value;
            }
            else if(i<3)
            {
                enemySlot.visible = false;
                enemySlot.SetEnabled(false);
            }

        }

    }
}
