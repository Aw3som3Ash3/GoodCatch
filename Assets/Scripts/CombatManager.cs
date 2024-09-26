using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Collections;
using UnityEditor.Rendering;
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
    Team currentTurnTeam;
    //Turn currentTurn;
    int currentTurn;
    int roundNmber;

    [SerializeField]
    CombatUI ui;
    [SerializeField]
    TurnListUI turnListUI;
    [SerializeField]
    CombatVisualizer combatVisualizer;


    [SerializeField]
    FishMonsterType testType;
    
    List<FishMonster> playerFishes=new List<FishMonster>(), enemyFishes = new List<FishMonster>();

    [SerializeField]
    CombatDepth shallows, middle, abyss;
    Dictionary<FishMonster,CombatDepth> fishCurrentDepth=new Dictionary<FishMonster,CombatDepth>();

    [SerializeField]
    Transform shallowsLocation, middleLocation, abyssLocation;
    //Dictionary<CombatDepth,Transform> depthTransform = new Dictionary<CombatDepth,Transform>();

    FishMonster selectedFish;
    CombatDepth targetedDepth;
    List<Turn> turnList=new List<Turn>();

   

    public Action IsTargeting;
    Action hasTargeted;


    bool hasActionLeft;
    bool actionsCompleted;
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;
    private void Awake()
    {
        shallows = new CombatDepth(Depth.shallow, shallowsLocation.GetChild(0), shallowsLocation.GetChild(1));
        middle = new CombatDepth(Depth.middle, middleLocation.GetChild(0), middleLocation.GetChild(1));
        abyss = new CombatDepth(Depth.abyss, abyssLocation.GetChild(0), abyssLocation.GetChild(1));


        //depthTransform[shallows] = shallowsLocation;
        //depthTransform[middle] = middleLocation;
        //depthTransform[abyss] = abyssLocation;
    }
    private void Start()
    {
       


        //temporary just for generating monsters right now
        //for (int i = 0; i < 3; i++)
        //{
        //    playerFishes.Add(testType.GenerateMonster());

        //    enemyFishes.Add(testType.GenerateMonster());
        //}
       

    }

    //void Set
    public void NewCombat(List<FishMonster> playerFishes, List<FishMonster> enemyFishes)
    {
        this.playerFishes = playerFishes;
        this.enemyFishes= enemyFishes;
        SetUp();
        StartTurn();
    }
    void SetUp()
    {
        virtualCamera.Priority = 11;
        ui.MoveAction += Move;
        ui.Ability += UseAbility;
        ui.EndTurn += NextTurn;
        ui.DepthSelection += TargetDepth;

        AddFish(playerFishes[0], shallows, Team.player);
        AddFish(playerFishes[1], middle, Team.player);
        AddFish(playerFishes[2], abyss, Team.player);

        AddFish(enemyFishes[0], shallows, Team.enemy);
        AddFish(enemyFishes[1], middle, Team.enemy);
        AddFish(enemyFishes[2], abyss, Team.enemy);


        for (int i = 0; i < 3; i++)
        {
            turnList.Add(new Turn(playerFishes[i],Team.player));

            turnList.Add(new Turn(enemyFishes[i],Team.enemy));
        }
        
        OrderTurn();
        turnListUI.SetTurnBar(turnList);
    }

    void AddFish(FishMonster fish,CombatDepth destination,Team team)
    {
        destination.AddFish(fish, team);
        fishCurrentDepth[fish] = destination;
        combatVisualizer.AddFish(fish, destination.GetSideTransform(team).position);
    }
    void OrderTurn()
    {
        turnList.Sort(SortBySpeed);
        currentTurn = 0;
       
    }
    int SortBySpeed(Turn x, Turn y)
    {
        return  x.speed - y.speed;
    }
    void ActionsCompleted()
    {
        actionsCompleted = true;
    }
    void StartTurn()
    {
        
        selectedFish = turnList[currentTurn].fish;
        ui.SetTurnMarker(combatVisualizer.fishToObject[selectedFish].transform);
        hasActionLeft = true;
        if (playerFishes.Contains(selectedFish))
        {
            //player
            
            currentTurnTeam = Team.player;
            ui.EnableButtons();
            ui.UpdateVisuals(selectedFish);
        }
        else
        {
            
            currentTurnTeam = Team.enemy;
            ui.DisableButtons();
            //Oponent Decision
            //temp next turn for right now just to skip the enemy
            NextTurn();
        }
    }
 void NextTurn() 
    {
        if (!actionsCompleted && turnList[currentTurn].team==Team.player)
        {
            return;
        }
        currentTurn++;
        if (currentTurn >= turnList.Count)
        {
            //OrderTurn();
            currentTurn = 0;
            roundNmber++;
        }
        turnListUI.UpdateTurns(currentTurn);
        StartTurn();
    }
    void ChangeDepth(FishMonster fish, CombatDepth destination)
    {
        if(destination== fishCurrentDepth[fish])
        {
            return;
        }
        actionsCompleted = false;
        var prevDepth = fishCurrentDepth[fish];
        prevDepth.RemoveFish(fish);
        destination.AddFish(fish, currentTurnTeam);
        fishCurrentDepth[fish] = destination;
        hasTargeted -=ChangingDepth;
        ui.StopTargeting();

        combatVisualizer.MoveFish(fish, destination.GetPositionOfFish(fish), ActionsCompleted);
        foreach (FishMonster t in prevDepth.player)
        {
              combatVisualizer.MoveFish(t, prevDepth.GetPositionOfFish(t));
        }
        foreach (FishMonster t in prevDepth.enemy)
        {
            combatVisualizer.MoveFish(t, prevDepth.GetPositionOfFish(t));
        }
        hasActionLeft = false;
        print("new destination: " + fishCurrentDepth[fish]);
        //ui.SetTurnMarker(fishRepresentation[selectedFish].transform);
    }
    void ChangingDepth()
    {
        ChangeDepth(selectedFish, targetedDepth);
        
    }
    public void Move()
    {
        //changingDepth;
        if (!hasActionLeft)
        {
            return;
        }
        
        hasTargeted =ChangingDepth;
        ui.StartTargeting();
        

    }
   
    public void UseAbility(int index)
    {
        if (!hasActionLeft)
        {
            return;
        }
        actionsCompleted = false;
        //Ability abilityToUse =;
        if (!selectedFish.GetAbility(index).CanUse(fishCurrentDepth[selectedFish].depth))
        {
            print("cannot use ability in this depth");
            return;
        }
        FishMonster[] targets = new FishMonster[3];
        if (selectedFish.GetAbility(index).Targeting==Ability.TargetingType.all )
        {
            Team targetedTeam= currentTurnTeam==Team.player ? Team.enemy : currentTurnTeam;
            targets[0] = shallows.TargetFirst(targetedTeam);
            targets[1] = middle.TargetFirst(targetedTeam);
            targets[2] = abyss.TargetFirst(targetedTeam);
            selectedFish.UseAbility(index, targets);
            hasActionLeft = false;
            ActionsCompleted();
        }
        else if(selectedFish.GetAbility(index).Targeting == Ability.TargetingType.single)
        {
            //target
            hasTargeted = ()=> ConfirmAttack(index);
            ui.StartTargeting(selectedFish.GetAbility(index).TargetableDepths);
        }
    }
    void ConfirmAttack(int index)
    {
        if (selectedFish.GetAbility(index).DepthTargetable(targetedDepth.depth))
        {
            Team targetedTeam = currentTurnTeam == Team.player ? Team.enemy : currentTurnTeam;
            selectedFish.UseAbility(index,targetedDepth.TargetFirst(targetedTeam));
            ui.StopTargeting();
            hasTargeted=null;
            hasActionLeft = false;
            ActionsCompleted();
        }
        else
        {
            print("can't target");
        }
        
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
        print("targeted "+ targetedDepth);
        hasTargeted?.Invoke();
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
   
    [Serializable]
    public class CombatDepth
    {
        
        public Depth depth { get; private set; }
        public ObservableCollection<FishMonster> player { get; private set; } = new ObservableCollection<FishMonster>();
        public ObservableCollection<FishMonster> enemy { get; private set; } = new ObservableCollection<FishMonster>();
        Dictionary<FishMonster, GameObject> fishObject;
        Transform playerSide;
        Transform enemySide;
       

        public CombatDepth(Depth depth, Transform playerSide, Transform enemySide)
        {
            this.depth = depth;
            this.playerSide = playerSide;
            this.enemySide = enemySide;
        }
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
            player.Remove(fish);
            enemy.Remove(fish);
            
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
        public Transform GetSideTransform(Team team)
        {
            if (team == Team.player)
            {
                return playerSide;
            }
            else
            {
                return enemySide;
            }
        }
        public Vector3 GetPositionOfFish(FishMonster fish)
        {
            if (player.Contains(fish))
            {
                print(player.IndexOf(fish));
                return  (player.IndexOf(fish) * Vector3.left * 1.5f) +playerSide.position;
            }else if (enemy.Contains(fish))
            {
                return (enemy.IndexOf(fish) * Vector3.right * 1.5f) + enemySide.position;
            }
            return Vector3.zero;
        }
    }
    [Serializable]
    public class Turn
    {
        public Team team { get; private set; }
        int actionsPerTurn=1,actionsLeft;
        public bool ActionLeft { get { return actionsLeft > 0; } }
        public FishMonster fish { get; private set; }
        //CombatDepth currentDepth;
        public int speed{ get { return fish.speed; } }
        public Turn(FishMonster fish, Team team, int actionsPerTurn=1)
        {
            this.team = team;
            this.fish = fish;
            this.actionsPerTurn = actionsPerTurn;

        }
        public void NewTurn()
        {
            actionsLeft = actionsPerTurn;

        }
        public void StartTurn()
        {
            
        }

        public void EndTurn()
        {

        }

        public void Move()
        {
            
        }

    }
}



//public abstract class TurnAction
//{

//}

//public class ChangeDepthAction : TurnAction
//{

//}
