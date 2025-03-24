using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewPartyUI : PausePage
{
    FishProfile[] fishProfiles = new FishProfile[6];
    Fishventory fishventory;
    ViewMonsters viewMonster;
    public NewPartyUI()
    {

        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/PartyUI");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.StretchToParentSize();
        this.focusable = true;
        this.delegatesFocus = true;
        viewMonster = this.Q<ViewMonsters>();
        viewMonster.visible = false;

        for (int i = 0; i < fishProfiles.Length; i++)
        {
            fishProfiles[i] = this.Q<FishProfile>("FishProfile" + (i + 1));
            int index = i;
            fishProfiles[i].RegisterCallback<ClickEvent>(evt => OpenFishInfo(index));
        }

    }
    public void UpdateUI()
    {
        fishventory = GameManager.Instance.PlayerFishventory;
        for (int i = 0; i < fishProfiles.Length; i++)
        {
            if (i < fishventory.Fishies.Count)
            {
                fishProfiles[i].SetFish(fishventory.Fishies[i]);
                fishProfiles[i].visible = true;
            }
            else
            {
                fishProfiles[i].visible = false;
            }
        }
    }
    private void OpenFishInfo(int index)
    {
        viewMonster.SetFish(fishventory.Fishies[index]);
        viewMonster.visible = true; 

    }
    public override bool Back()
    {
        if (viewMonster.visible)
        {
            viewMonster.Close();

            viewMonster.visible = false;
            return false;
        }
        return base.Back();
    }
}
