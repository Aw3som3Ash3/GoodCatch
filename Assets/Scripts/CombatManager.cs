using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static CombatManager;
using static UnityEngine.Rendering.DebugUI;

public class CombatManager : MonoBehaviour
{
    public enum Team 
    {
        player,
        enemy
    }
    bool isTargeting;
    Team currentTurnTeam;
    //Turn currentTurn;
    int currentTurn;
    int roundNmber;

    [SerializeField]
    CombatUI ui;

    [SerializeField]
    FishMonsterType testType;
    
    List<FishMonster> playerFishes=new List<FishMonster>(), enemyFishes = new List<FishMonster>();

    Dictionary<FishMonster, GameObject> fishRepresentation=new Dictionary<FishMonster, GameObject>();
    [SerializeField]
    CombatDepth shallows, middle, abyss;
    Dictionary<FishMonster,CombatDepth> fishCurrentDepth=new Dictionary<FishMonster,CombatDepth>();

    [SerializeField]
    Transform shallowsLocation, middleLocation, abyssLocation;
    Dictionary<CombatDepth,Transform> depthTransform = new Dictionary<CombatDepth,Transform>();

    FishMonster selectedFish;
    CombatDepth targetedDepth;
    List<FishMonster> turnList=new List<FishMonster>();

    Ability abilityToUse;

    public Action IsTargeting;
    Action hasTargeted;
    
   // delegate void ChangingDepth();
  
    EventHandler changingDepth;
    private void Start()
    {
        //shallows = new CombatDepth(Depth.shallow, shallowsLocation.GetChild(0), shallowsLocation.GetChild(1));
        //middle = new CombatDepth(Depth.middle, middleLocation.GetChild(0), middleLocation.GetChild(1));
        //abyss = new CombatDepth(Depth.abyss, abyssLocation.GetChild(0), abyssLocation.GetChild(1));
        depthTransform[shallows] = shallowsLocation;
        depthTransform[middle] = middleLocation;
        depthTransform[abyss] = abyssLocation;


        InputManager.Input.Enable();
        for (int i = 0; i < 3; i++)
        {
            playerFishes.Add(testType.GenerateMonster());

            enemyFishes.Add(testType.GenerateMonster());
        }
        SetUp();
        StartTurn();

    }
    void SetUp()
    {

        ui.MoveAction += Move;
        ui.Ability += UseAbility;
        
        shallows.AddFish(playerFishes[0], Team.player);
        fishCurrentDepth[playerFishes[0]] = shallows;

        middle.AddFish(playerFishes[1],Team.player);
        fishCurrentDepth[playerFishes[1]] = middle;

        abyss.AddFish(playerFishes[2], Team.player);
        fishCurrentDepth[playerFishes[2]] = abyss;

        ////
        ////
        shallows.AddFish(enemyFishes[0], Team.enemy);
        fishCurrentDepth[enemyFishes[0]] = shallows;

        middle.AddFish(enemyFishes[1], Team.enemy);
        fishCurrentDepth[enemyFishes[1]] = middle;

        abyss.AddFish(enemyFishes[2], Team.enemy);
        fishCurrentDepth[enemyFishes[2]] = abyss;
        for (int i = 0; i < 3; i++)
        {
            turnList.Add(playerFishes[i]);
            
            fishRepresentation[playerFishes[i]] = Instantiate(playerFishes[i].Model, depthTransform[fishCurrentDepth[playerFishes[i]]].GetChild(0));


            turnList.Add(enemyFishes[i]);
            
            fishRepresentation[enemyFishes[i]] = Instantiate(enemyFishes[i].Model, depthTransform[fishCurrentDepth[enemyFishes[i]]].GetChild(1));
        }
      
        OrderTurn();
    }
    void OrderTurn()
    {
        turnList.Sort(SortBySpeed);
        currentTurn = 0;
       
    }
    int SortBySpeed(FishMonster x, FishMonster y)
    {
        return  x.speed - y.speed;
    }
    void NextTurn() 
    {
        currentTurn++;
        if (currentTurn >= turnList.Count)
        {
            //OrderTurn();
            currentTurn = 0;
            roundNmber++;
        }
        StartTurn();
    }
    void StartTurn()
    {
        selectedFish = turnList[currentTurn];
        isTargeting = false;
        ui.SetTurnMarker(fishRepresentation[selectedFish].transform);
        if (playerFishes.Contains(selectedFish))
        {
            //player
            
            currentTurnTeam = Team.player;
            ui.EnableButtons();
        }
        else
        {
            
            currentTurnTeam = Team.enemy;
            ui.DisableButtons();
            //Oponent Decision
            NextTurn();
        }
    }

    void ChangeDepth(FishMonster fish, CombatDepth destination)
    {
        var prevDepth = fishCurrentDepth[fish];
        prevDepth.RemoveFish(fish);
        destination.AddFish(fish, currentTurnTeam);
        fishCurrentDepth[fish] = destination;
        hasTargeted -=ChangingDepth;
        StopTargeting();
        fishRepresentation[fish].transform.parent = depthTransform[destination].GetChild(currentTurnTeam==Team.player? 0:1).transform;
        fishRepresentation[fish].transform.localPosition = Vector3.zero + Vector3.left * fishRepresentation[fish].transform.GetSiblingIndex() * (currentTurnTeam == Team.player ? 1.5f : -1.5f);
        foreach (Transform t in depthTransform[prevDepth].GetChild(currentTurnTeam == Team.player ? 0 : 1))
        {
            t.localPosition= Vector3.zero + Vector3.left * t.transform.GetSiblingIndex() * (currentTurnTeam == Team.player ? 1.5f : -1.5f);
        }
        
        print("new destination: " + fishCurrentDepth[fish]);

        NextTurn();
    }
    void ChangingDepth()
    {
        ChangeDepth(selectedFish, targetedDepth);
        hasTargeted -= ChangingDepth;
    }
    public void Move()
    {
        //changingDepth;
        
        
        hasTargeted =ChangingDepth;
        StartTargeting();
        

    }
    void StartTargeting()
    {
        isTargeting = true;
        InputManager.Input.UI.Click.performed += OnClick;
    }
    void StopTargeting()
    {
        isTargeting = false;
        InputManager.Input.UI.Click.performed -= OnClick;
        

    }
    public void UseAbility(int index)
    {
      
        abilityToUse =selectedFish.GetAbility(index);
        FishMonster[] targets = new FishMonster[3];
        if (abilityToUse.Targeting==Ability.TargetingType.all )
        {
            Team targetedTeam= currentTurnTeam==Team.player ? Team.enemy : currentTurnTeam;
            targets[0] = shallows.TargetFirst(targetedTeam);
            targets[1] = middle.TargetFirst(targetedTeam);
            targets[2] = abyss.TargetFirst(targetedTeam);
            selectedFish.UseAbility(index, targets);
            NextTurn();
        }
        else if(abilityToUse.Targeting == Ability.TargetingType.single)
        {
            //target
            hasTargeted = ConfirmAttack;
            StartTargeting();
        }
    }
    void ConfirmAttack()
    {
        Team targetedTeam = currentTurnTeam == Team.player ? Team.enemy : currentTurnTeam;
        abilityToUse.UseAbility(targetedDepth.TargetFirst(targetedTeam));
        StopTargeting();
        hasTargeted -= ConfirmAttack;
        NextTurn();
    }
    void TargetDepth(int index)
    {
        switch (index)
        {
            case 0:
                targetedDepth = shallows;
                break;
            case 1:
                targetedDepth = middle;
                break;
            case 2:
                targetedDepth = abyss;
                break;
            default:
                targetedDepth = null;
                break;
        }
        //selectedFish.UseAbility(index, targetedDepth.TargetFirst());
        //targetedDepth = null;
    }


    void TargetDepth(CombatDepth target)
    {
        targetedDepth = target;
    }


   
    private void Update()
    {
        
    }
    void OnClick(InputAction.CallbackContext context)
    {
        RaycastHit2D hit= Physics2D.Raycast(Camera.main.ScreenToWorldPoint(InputManager.Input.UI.Point.ReadValue<Vector2>()), (Vector2)Camera.main.ScreenToWorldPoint(InputManager.Input.UI.Point.ReadValue<Vector2>()) + Vector2.right);
        if (hit)
        {
            TargetDepth(hit.collider.transform.GetSiblingIndex());
            print(targetedDepth);
            hasTargeted.Invoke();
        }
    }
    [Serializable]
    public class CombatDepth
    {
        
        Depth depth;
        ObservableCollection<FishMonster> player=new ObservableCollection<FishMonster>();
        ObservableCollection<FishMonster> enemy=new ObservableCollection<FishMonster>();
        //Dictionary<FishMonster, GameObject> fishObject;
        //Transform playerSide; 
        //Transform enemySide;

        //public CombatDepth(Depth depth,Transform playerSide,Transform enemySide)
        //{
        //    this.depth = depth;
        //}
        public void AddFish(FishMonster fish,Team team)
        {
            //this.fishObject[fish] = fishObject;
            if (team == Team.player)
            {
                player.Add(fish);
            }
            else
            {
                enemy.Add(fish);
            }
           
        }
        public void RemoveFish(FishMonster fish)
        {
            if (!player.Remove(fish))
            {
                enemy.Remove(fish);

            }
            else
            {

            }
        }
        public FishMonster TargetFirst(Team team)
        {
            if (team == Team.player)
            {
                return player[0];
            }
            else
            {
                return enemy[0];
            }
           
        }
        public void SwapFish(int index,Team team)
        {
            if (team == Team.player)
            {
                player.Move(index,0);
            }
            else
            {
                enemy.Move(index, 0);
            }
        }
    }
}




//public abstract class TurnAction
//{

//}

//public class ChangeDepthAction : TurnAction
//{

//}
