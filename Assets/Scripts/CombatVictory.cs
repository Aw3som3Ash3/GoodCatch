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
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/CombatVictory.uxml");
        visualTreeAsset.CloneTree(root);
    }

    public CombatVictory(List<FishMonster> playerFishes,List<FishMonster>enemyFishes)
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/CombatVictory.uxml");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        for(int i = 0; i < playerFishes.Count; i++)
        {
            var fishslot = this.Q("Fish" + (i + 1));
            fishslot.Q<ProgressBar>("healthBar").value = playerFishes[i].Health;
            fishslot.Q<ProgressBar>("healthBar").highValue = playerFishes[i].MaxHealth;
            fishslot.Q<ProgressBar>("xpBar").value = playerFishes[i].Xp;
            fishXpBar[playerFishes[i]] = fishslot.Q<ProgressBar>("xpBar");
            fishslot.Q("ProfilePic").style.backgroundImage = playerFishes[i].Icon;
        }

    }
}
