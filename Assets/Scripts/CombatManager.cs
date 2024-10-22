using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static CombatManager;

public class CombatManager : MonoBehaviour
{
    public enum Team 
    {
        player,
        enemy
    }
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
    //Dictionary<FishMonster,CombatDepth> fishCurrentDepth=new Dictionary<FishMonster,CombatDepth>();

    [SerializeField]
    Transform shallowsLocation, middleLocation, abyssLocation;
    //Dictionary<CombatDepth,Transform> depthTransform = new Dictionary<CombatDepth,Transform>();

    //FishMonster selectedFish;
    //CombatDepth targetedDepth;
    List<Turn> turnList=new List<Turn>();
    Dictionary<FishMonster, Turn> getFishesTurn=new Dictionary<FishMonster, Turn>();
    
    
    //public Action IsTargeting;
    //Action hasTargeted;


   // bool hasActionLeft;
    bool actionsCompleted;
    Action CompletedAllActions;
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
   /// <summary>
   /// tells the combat manager to setup a new combat with a set of paramaters
   /// <param name="playerFishes"></param>
   /// <param name="enemyFishes"></param>
   /// <param name="rewardFish"></param>
    public void NewCombat(List<FishMonster> playerFishes, List<FishMonster> enemyFishes,bool rewardFish=false)
    {
        this.playerFishes = playerFishes;
        this.enemyFishes= enemyFishes;
        this.rewardFish = rewardFish;
        SetUp();
        StartTurn();
        
    }
    //sets up the combat
    void SetUp()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        prevCam = Camera.main;
        prevCam.gameObject.SetActive(false);
        cam.enabled = true;
        Camera.SetupCurrent(cam);
        //ui.MoveAction += ChangeDepth;
       // ui.AbilityAction += UseAbility;
        Turn.TurnEnded += NextTurn;

        for(int i = 0; i < playerFishes.Count; i++)
        {
           
            switch (i % 3)
            {
                case 0:
                    AddFish(playerFishes[i], shallows, Team.player);
                    
                    break;
                case 1:
                    AddFish(playerFishes[i], middle, Team.player);
                    break;
                case 2:
                    AddFish(playerFishes[i], abyss, Team.player);
                    break;

            }
            Turn turn = new Turn(this, playerFishes[i], Team.player, GetDepth(i % 3));
            turn.OnMove = DepthChanged;
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
            Turn turn = new Turn(this, enemyFishes[i], Team.enemy, GetDepth(i % 3));
            turn.OnMove = DepthChanged;
            turnList.Add(turn);
            getFishesTurn[enemyFishes[i]] = turn;
        }

        
        OrderTurn();
        
    }

    void AddFish(FishMonster fish,CombatDepth destination,Team team)
    {

        destination.AddFish(fish, team);
        //fishCurrentDepth[fish] = destination;
        combatVisualizer.AddFish(fish, destination.GetSideTransform(team).position,team);
        combatVisualizer.MoveFish(fish, shallows.GetPositionOfFish(fish));
        fish.HasFeinted += () => RemoveFishFromBattle(fish);

    }
    //orders the turns by speed value
    void OrderTurn()
    {
        foreach (var turn in turnList)
        {
            turn.RollInitiative();
        }
        turnList.Sort(SortBySpeed);
        currentTurn = 0;
        turnListUI.SetTurnBar(turnList);
    }
    int SortBySpeed(Turn x, Turn y)
    {
        return  x.initiative - y.initiative;
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
        CompletedAllActions?.Invoke();
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
        


        //selectedFish = turnList[currentTurn].fish;
        ui.SetTurnMarker(combatVisualizer.fishToObject[turnList[currentTurn].fish].transform);
        turnList[currentTurn].StartTurn();
        //ui.UpdateVisuals(turnList[currentTurn]);
        //if (playerFishes.Contains(selectedFish))
        //{
        //    //player
            
            
        //    ui.EnableButtons();
          
        //}
        //else
        //{
        //    ui.DisableButtons();
        //    //Oponent Decision
        //    //temp next turn for right now just to skip the enemy
        //    Invoke("EnemyLogic",0.5f);
        //}
    }

    void EnemyLogic()
    {
        //CompletedAllActions = NextTurn;
        //UseAbility(0,0);
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
            currentTurn = 0;
            roundNmber++;
        }
        turnListUI.UpdateTurns(currentTurn);
        //selectedFish.RecoverStamina();
        StartTurn();
    }
    //void ChangeDepth(FishMonster fish, int destinationIndex)
    //{
    //    CombatDepth destination=GetDepth(destinationIndex);
    //    if(destination== fishCurrentDepth[fish])
    //    {
    //        return;
    //    }
    //    actionsCompleted = false;
    //    var prevDepth = fishCurrentDepth[fish];
    //    prevDepth.RemoveFish(fish);
    //    destination.AddFish(fish, currentTurnTeam);
    //    fishCurrentDepth[fish] = destination;
    //    combatVisualizer.MoveFish(fish, destination.GetPositionOfFish(fish), ActionsCompleted);
    //    foreach (FishMonster t in prevDepth.player)
    //    {
    //          combatVisualizer.MoveFish(t, prevDepth.GetPositionOfFish(t));
    //    }
    //    foreach (FishMonster t in prevDepth.enemy)
    //    {
    //        combatVisualizer.MoveFish(t, prevDepth.GetPositionOfFish(t));
    //    }
    //    turnList[currentTurn].UseAction();
    //    ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
    //    //ui.EnableButtons();
    //    print("new destination: " + fishCurrentDepth[fish]);
    //}

    void DepthChanged(Turn turn,CombatDepth previous,CombatDepth destination)
    {
      
        combatVisualizer.MoveFish(turn.fish, destination.GetPositionOfFish(turn.fish),ActionsCompleted);
        if (previous != null)
        {
            foreach (FishMonster t in previous.player)
            {
                combatVisualizer.MoveFish(t, previous.GetPositionOfFish(t));
            }
            foreach (FishMonster t in previous.enemy)
            {
                combatVisualizer.MoveFish(t, previous.GetPositionOfFish(t));
            }
        }
        
    }

   
    void UseAbility(Turn turn,int index,int depthIndex)
    {
        Team targetedTeam = Team.player;
        if (turn.fish.GetAbility(index).TargetedTeam == Ability.TargetTeam.friendly)
        {
            targetedTeam = turn.team;
        }
        else if (turn.fish.GetAbility(index).TargetedTeam == Ability.TargetTeam.enemy)
        {
            targetedTeam = turn.team == Team.player ? Team.enemy : Team.player;
        }
        actionsCompleted = false;
        if (!turn.fish.GetAbility(index).CanUse(turn.currentDepth.depth))
        {
            print("cannot use ability in this depth");
            return;
        }
        FishMonster[] targets = new FishMonster[3];
        if (turn.fish.GetAbility(index).Targeting==Ability.TargetingType.all )
        {
            
            targets[0] = shallows.TargetFirst(targetedTeam);
            targets[1] = middle.TargetFirst(targetedTeam);
            targets[2] = abyss.TargetFirst(targetedTeam);
            turn.fish.UseAbility(index, targets);

            foreach (var target in targets)
            {
                if (target != null)
                {
                    combatVisualizer.AnimateAttack(turn.fish, target, ActionsCompleted);
                }
            }
        }
        else if(turn.fish.GetAbility(index).Targeting == Ability.TargetingType.single)
        {
            
            CombatDepth targetedDepth = GetDepth(depthIndex);

            FishMonster targetedFish = targetedDepth.TargetFirst(targetedTeam);
            if (targetedFish == null)
            {
                ActionsCompleted();
                return;
            }
            //ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
            var attackingFish = turn.fish;
            combatVisualizer.AnimateAttack(turn.fish, targetedFish, () => { attackingFish.UseAbility(index, targetedFish); ActionsCompleted(); });
        }
    }
    CombatDepth GetDepth(int index)
    {
        switch (index)
        {
            case 0:
                return shallows;
            case 1:
                return middle;
            case 2:
                return abyss;
            default:
                return null;
        }

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

    //void TargetDepth(CombatDepth target)
    //{
    //    targetedDepth = target;
    //}


   
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
            else if(team == Team.enemy&&enemy.Count>0)
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

        public bool ActionLeft { get { return actionsLeft > 0 && actionsCompleted; } }
        public FishMonster fish { get; private set; }
        public CombatDepth currentDepth { get; private set; }
        CombatManager combatManager;
        public int initiative{ get; private set; }

        public Action<Turn, CombatDepth, CombatDepth> OnMove;
        public static Action<Turn,bool> NewTurn;
        public static Action TurnEnded;
        bool actionsCompleted=true;

        public float Health { get { return fish.health; } }
        public float MaxHealth { get { return fish.maxHealth; } }

        public float maxStamina { get { return fish.maxStamina; }}
        public float stamina { get; private set; }
        public Turn(CombatManager combatManager, FishMonster fish, Team team,CombatDepth startingDepth ,int actionsPerTurn=2)
        {
            this.team = team;
            this.fish = fish;
            this.actionsPerTurn = actionsPerTurn;
            this.combatManager = combatManager;
            currentDepth = startingDepth;
            combatManager.CompletedAllActions += ActionsCompleted;
            stamina = maxStamina;
        }
        public void StartTurn()
        {
            actionsLeft = actionsPerTurn;
            NewTurn?.Invoke(this,team==Team.player);
            if (team == Team.enemy)
            {
                UseAbility(0, 0);
                EndTurn();
            }
        }
        void ActionsCompleted()
        {
            actionsCompleted = true;
        }
        public void UseAction()
        {
            actionsLeft--;
        }
        public void RollInitiative()
        {
            initiative = UnityEngine.Random.Range(0, 20) + fish.agility;
        }
        public void EndTurn()
        {
            TurnEnded?.Invoke();
        }
        public void SetDepth(CombatDepth depth)
        {
            this.currentDepth = depth;
        }
        public void Move(int depthIndex)
        {
            actionsCompleted = false;
            CombatDepth prevDepth=null;
            if (currentDepth != null)
            {
                prevDepth = currentDepth;
                prevDepth.RemoveFish(fish);
            }
            UseAction();
            currentDepth = combatManager.GetDepth(depthIndex);

            currentDepth.AddFish(fish, team);
            OnMove?.Invoke(this, prevDepth, currentDepth);

        }
        public bool AbilityUsable(int abilityIndex)
        {
            var ability = fish.GetAbility(abilityIndex);
           
            return ability.StaminaUsage<stamina && ability.AvailableDepths.HasFlag(currentDepth.depth);
        }
        public void UseAbility(int abilityIndex,int target)
        {
            actionsCompleted = false;
            CombatDepth targetDepth = combatManager.GetDepth(target);
            combatManager.UseAbility(this,abilityIndex, target);
            UseAction();
        }

    }
}



