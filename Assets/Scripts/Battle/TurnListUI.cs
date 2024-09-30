using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class TurnListUI : MonoBehaviour
{
    [SerializeField]
    RectTransform turnBar;
    [SerializeField]
    GameObject turnPreviewPrefab;
    ObservableCollection<FishIcon> turnIcons=new ObservableCollection<FishIcon>();
    Dictionary<CombatManager.Turn, FishIcon> turnToIcon=new Dictionary<CombatManager.Turn, FishIcon>();
    //List<FishMonster> fishesInTurn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTurnBar(List<CombatManager.Turn> turns)
    {
        foreach (CombatManager.Turn turn in turns)
        {
            FishIcon turnIcon= Instantiate(turnPreviewPrefab, turnBar).GetComponent<FishIcon>();
            turnIcon.SetIcon(turn.fish.Icon,turn.team==CombatManager.Team.player ? Color.green:Color.red);
            turnToIcon[turn] = turnIcon;
            turnIcons.Add(turnIcon);
           
        }
    }
    public void UpdateTurns(int currentTurn)
    {
        Debug.Log("updating turn ui");
        turnIcons[0].transform.SetAsLastSibling();
        turnIcons.Move(0, turnIcons.Count-1);
        
    }
}
