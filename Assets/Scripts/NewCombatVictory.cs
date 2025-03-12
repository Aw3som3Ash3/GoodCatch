using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NewCombatVictory : VisualElement
{

    public Dictionary<FishMonster, FishProfile> fishProfile { get; private set; } = new();
    public new class UxmlFactory : UxmlFactory<CombatVictory, CombatVictory.UxmlTraits>
    {


    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public NewCombatVictory()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ResultsCombatPage");

        visualTreeAsset.CloneTree(root);
    }

    public NewCombatVictory(List<FishMonster> playerFishes,Dictionary<FishMonster,bool> fishesCaught=null)
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/ResultsCombatPage");

        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        Debug.Log(playerFishes.Count);
        for (int i = 0; i < 6; i++)
        {
            if (i < playerFishes.Count)
            {
                Debug.Log(playerFishes[i]);
                var profile = this.Q<FishProfile>("FishProfile" + (i + 1));
                profile.SetFish(playerFishes[i]);
                fishProfile[playerFishes[i]] = profile;
            }
            else
            {
                this.Q<FishProfile>("FishProfile" + (i + 1)).visible = false;
            }
        }
           
    }


        

    
}
