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
    ObservableCollection<TurnIcon> turnIcons=new ObservableCollection<TurnIcon>();
    Dictionary<CombatManager.Turn, TurnIcon> turnToIcon=new Dictionary<CombatManager.Turn, TurnIcon>();
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
            TurnIcon turnIcon= Instantiate(turnPreviewPrefab, turnBar).GetComponent<TurnIcon>();
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
