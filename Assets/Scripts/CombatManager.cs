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
using UnityEngine.SceneManagement;
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
    Dictionary<FishMonster, Turn> getFishesTurn=new Dictionary<FishMonster, Turn>();
    

    public Action IsTargeting;
    Action hasTargeted;


   // bool hasActionLeft;
    bool actionsCompleted;
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    Camera cam;

    Camera prevCam;
    bool rewardFish;
    private void Awake()
    {
        shallows = new CombatDepth(Depth.shallow, shallowsLocation.GetChild(0), shallowsLocation.GetChild(1));
        middle = new CombatDepth(Depth.middle, middleLocation.GetChild(0), middleLocation.GetChild(1));
        abyss = new CombatDepth(Depth.abyss, abyssLocation.GetChild(0), abyssLocation.GetChild(1));



    }
    private void Start()
    {

       

    }
   
    //void Set
    public void NewCombat(List<FishMonster> playerFishes, List<FishMonster> enemyFishes,bool rewardFish=false)
    {
        this.playerFishes = playerFishes;
        this.enemyFishes= enemyFishes;
        this.rewardFish = rewardFish;
        SetUp();
        StartTurn();
        
    }
    void SetUp()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        prevCam = Camera.main;
        prevCam.gameObject.SetActive(false);
        cam.enabled = true;
        Camera.SetupCurrent(cam);
        //virtualCamera.Priority = 11;
        ui.MoveAction += Move;
        ui.Ability += UseAbility;
        ui.EndTurn += NextTurn;
        ui.DepthSelection += TargetDepth;

        for(int i = 0; i < playerFishes.Count; i++)
        {
            
            switch (i % 3)
            {
                case 0:
                    AddFish(playerFishes[i], shallows, Team.player);
                    combatVisualizer.MoveFish(playerFishes[i], shallows.GetPositionOfFish(playerFishes[i]));
                    break;
                case 1:
                    AddFish(playerFishes[i], middle, Team.player);
                    combatVisualizer.MoveFish(playerFishes[i], middle.GetPositionOfFish(playerFishes[i]));
                    break;
                case 2:
                    AddFish(playerFishes[i], abyss, Team.player);
                    combatVisualizer.MoveFish(playerFishes[i], abyss.GetPositionOfFish(playerFishes[i]));
                    break;

            }
            Turn turn = new Turn(playerFishes[i], Team.player);
            turnList.Add(turn);
            getFishesTurn[playerFishes[i]]=turn;
            
        }
        for (int i = 0; i < enemyFishes.Count; i++)
        {
            switch (i % 3)
            {
                case 0:
                    AddFish(enemyFishes[i], shallows, Team.enemy);
                    break;
                case 1:
                    AddFish(enemyFishes[i], middle, Team.enemy);
                    break;
                case 2:
                    AddFish(enemyFishes[i], abyss, Team.enemy);
                    break;
            }
                Turn turn = new Turn(enemyFishes[i], Team.enemy);
            turnList.Add(turn);
            combatVisualizer.MoveFish(enemyFishes[i], shallows.GetPositionOfFish(enemyFishes[i]));

            getFishesTurn[enemyFishes[i]] = turn;
        }
        //if (playerFishes[0] != null)

        //if (playerFishes[1] != null)
        //    AddFish(playerFishes[1], middle, Team.player);
        //if (playerFishes[2] != null)
        //    AddFish(playerFishes[2], abyss, Team.player);

        //if (enemyFishes[0] != null)
        //    AddFish(enemyFishes[0], shallows, Team.enemy);
        //if (enemyFishes[1] != null)
        //    AddFish(enemyFishes[1], middle, Team.enemy);
        //if (enemyFishes[2] != null)
        //    AddFish(enemyFishes[2], abyss, Team.enemy);


        
        OrderTurn();
        turnListUI.SetTurnBar(turnList);
    }

    void AddFish(FishMonster fish,CombatDepth destination,Team team)
    {

        destination.AddFish(fish, team);
        fishCurrentDepth[fish] = destination;
        combatVisualizer.AddFish(fish, destination.GetSideTransform(team).position);
        fish.HasFeinted += () => RemoveFishFromBattle(fish);

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
        int amountDead=0;
        for(int i = 0; i < enemyFishes.Count; i++)
        {
            if (enemyFishes[i].isDead)
            {
                amountDead++;
            }
           

        }
        if (amountDead == enemyFishes.Count)
        {
            EndFight();
        }
    }

    void EndFight()
    {
        prevCam.gameObject.SetActive(true);
        Camera.SetupCurrent(prevCam);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("BattleScene"));
        if (rewardFish)
        {
            foreach(var fish in enemyFishes)
            {
                fish.RestoreAllHealth();
                GameManager.Instance.CapturedFish(fish);
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void StartTurn()
    {
        
        selectedFish = turnList[currentTurn].fish;
        ui.SetTurnMarker(combatVisualizer.fishToObject[selectedFish].transform);
        turnList[currentTurn].NewTurn();
        if (playerFishes.Contains(selectedFish))
        {
            //player
            
            currentTurnTeam = Team.player;
            ui.EnableButtons();
            ui.UpdateVisuals(turnList[currentTurn]);
        }
        else
        {
            
            currentTurnTeam = Team.enemy;
            ui.DisableButtons();
            ui.UpdateVisuals(turnList[currentTurn]);
            //Oponent Decision
            //temp next turn for right now just to skip the enemy
            Invoke("EnemyLogic",0.5f);
        }
    }

    void EnemyLogic()
    {
        UseAbility(0);
        targetedDepth = shallows;
        hasTargeted?.Invoke();
        //ConfirmAttack(0);
        NextTurn();
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
        selectedFish.RecoverStamina();
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
        turnList[currentTurn].UseAction();
        ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
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
        if (!turnList[currentTurn].ActionLeft)
        {
            return;
        }
        
        hasTargeted =ChangingDepth;
        ui.StartTargeting();
        

    }
   
    public void UseAbility(int index)
    {
        if (!turnList[currentTurn].ActionLeft)
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

            foreach (var target in targets)
            {
                if (target != null)
                {
                    combatVisualizer.AnimateAttack(selectedFish, target, ActionsCompleted);
                }
            }
                
            turnList[currentTurn].UseAction();
            ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
            //ActionsCompleted();
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
        Team targetedTeam = currentTurnTeam == Team.player ? Team.enemy : Team.player;
        if (selectedFish.GetAbility(index).DepthTargetable(targetedDepth.depth)&&targetedDepth.SideHasFish(targetedTeam))
        {
            
            FishMonster targetedFish = targetedDepth.TargetFirst(targetedTeam);
            
            ui.StopTargeting();
            hasTargeted=null;
            turnList[currentTurn].UseAction();
            ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
            var attackingFish = selectedFish;
            combatVisualizer.AnimateAttack(selectedFish, targetedFish, ()=>{ attackingFish.UseAbility(index, targetedFish); ActionsCompleted(); });
            
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

    void RemoveFishFromBattle(FishMonster fish)
    {
        turnList.Remove(getFishesTurn[fish]);
        shallows.RemoveFish(fish);
        middle.RemoveFish(fish);
        abyss.RemoveFish(fish);
        combatVisualizer.RemoveFish(fish);
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
       
        public bool SideHasFish(Team team)
        {
            if (team == Team.player)
            {
                return player.Count > 0;
            }
            else
            {
                return enemy.Count > 0;
            }
        }
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
            if (team == Team.player&&player.Count>0)
            {
                return player[0];
            }
            else if(enemy.Count>0)
            {
                return enemy[0];
            }
            else
            {
                return null;
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
        public int actionsPerTurn { get; private set; } = 1;
        public int actionsLeft { get; private set; }

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
        public void UseAction()
        {
            actionsLeft--;
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
