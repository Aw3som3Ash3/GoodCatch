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
    CombatDepth[] depths=new CombatDepth[3];
    Dictionary<Depth, CombatDepth> depth=new Dictionary<Depth, CombatDepth>();
    //Dictionary<FishMonster,CombatDepth> fishCurrentDepth=new Dictionary<FishMonster,CombatDepth>();
    Dictionary<CombatDepth, int> depthIndex=new Dictionary<CombatDepth, int>();

    [SerializeField]
    Transform shallowsLocation, middleLocation, abyssLocation;
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
        depths[0] = new CombatDepth(Depth.shallow, shallowsLocation.GetChild(0), shallowsLocation.GetChild(1));
        depth[Depth.shallow] = depths[0];
        depthIndex[depths[0]] = 0;
        depths[1] = new CombatDepth(Depth.middle, middleLocation.GetChild(0), middleLocation.GetChild(1));
        depth[Depth.middle] = depths[1];
        depthIndex[depths[1]] = 1;
        depths[2] = new CombatDepth(Depth.abyss, abyssLocation.GetChild(0), abyssLocation.GetChild(1));
        depth[Depth.abyss] = depths[2];
        depthIndex[depths[2]] = 2;

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
        Turn.TurnEnded += NextTurn;

        for(int i = 0; i < playerFishes.Count; i++)
        {
            Turn turn = new PlayerTurn(this, playerFishes[i],depths[i % 3]);
            AddFish(turn, depths[i % 3], Team.player);
            turnList.Add(turn);
            getFishesTurn[playerFishes[i]]=turn;
            
        }
        for (int i = 0; i < enemyFishes.Count; i++)
        {
            Turn turn = new EnemyTurn(this, enemyFishes[i], depths[i % 3]);
            AddFish(turn, depths[i % 3], Team.enemy);
            turnList.Add(turn);
            getFishesTurn[enemyFishes[i]] = turn;
        }

        
        OrderTurn();
        
    }

    void AddFish(Turn turn,CombatDepth destination,Team team)
    {
        
        destination.AddFish(turn, team);
        combatVisualizer.AddFish(turn.fish, destination.GetSideTransform(team).position,team);
        combatVisualizer.MoveFish(turn.fish, destination.GetPositionOfFish(turn));
        turn.HasFeinted += () => RemoveFishFromBattle(turn);

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
        ui.SetTurnMarker(combatVisualizer.fishToObject[turnList[currentTurn].fish].transform);
        turnList[currentTurn].StartTurn();
        
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

    void DepthChanged(Turn turn,CombatDepth previous,CombatDepth destination)
    {
      
        combatVisualizer.MoveFish(turn.fish, destination.GetPositionOfFish(turn),ActionsCompleted);
        if (previous != null)
        {
            foreach (Turn t in previous.player)
            {
                combatVisualizer.MoveFish(t.fish, previous.GetPositionOfFish(t));
            }
            foreach (Turn t in previous.enemy)
            {
                combatVisualizer.MoveFish(t.fish, previous.GetPositionOfFish(t));
            }
        }
        
    }

   
    void UseAbility(Turn turn,int index,int depthIndex)
    {
        Ability ability=turn.fish.GetAbility(index);
        if (!ability.CanUse(turn.currentDepth.depth))
        {
            print("cannot use ability in this depth");
            return;
        }
        Team targetedTeam = Team.player;
        if (ability.TargetedTeam == Ability.TargetTeam.friendly)
        {
            targetedTeam = turn.team;
        }
        else if (ability.TargetedTeam == Ability.TargetTeam.enemy)
        {
            targetedTeam = turn.team == Team.player ? Team.enemy : Team.player;
        }

        actionsCompleted = false;
        Turn[] targets = new Turn[3];
        if (turn.fish.GetAbility(index).Targeting==Ability.TargetingType.all )
        {
            targets[0] = depth[Depth.shallow].TargetFirst(targetedTeam);
            targets[1] = depth[Depth.middle].TargetFirst(targetedTeam);
            targets[2] = depth[Depth.abyss].TargetFirst(targetedTeam);
            
            foreach (var target in targets)
            {
                if (target != null)
                {
                    bool hit;
                    ability.UseAbility(turn.fish, target, out hit);
                    combatVisualizer.AnimateAttack(turn.fish, target.fish,()=> {  ActionsCompleted(); } );
                }
            }
        }
        else if(ability.Targeting == Ability.TargetingType.single)
        {
            CombatDepth targetedDepth = depths[depthIndex];
            Turn targetedFish = targetedDepth.TargetFirst(targetedTeam);
            if (targetedFish == null)
            {
                ActionsCompleted();
                return;
            }
            //ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
            // var attackingFish = turn.fish;
            bool hit;
            ability.UseAbility(turn.fish, targetedFish, out hit);
            combatVisualizer.AnimateAttack(turn.fish, targetedFish.fish, () => {  ActionsCompleted(); });
        }
    }
    void RemoveFishFromBattle(Turn turn)
    {
        turnList.Remove(turn);
        foreach(CombatDepth depth in depths)
        {
            depth.RemoveFish(turn);
        }
        combatVisualizer.RemoveFish(turn.fish);
    }
   
    private void Update()
    {

    }

    [Serializable]
    public class CombatDepth
    {
        
        public Depth depth { get; private set; }
        public ObservableCollection<Turn> player { get; private set; } = new ObservableCollection<Turn>();
        public ObservableCollection<Turn> enemy { get; private set; } = new ObservableCollection<Turn>();
        Dictionary<Turn, GameObject> fishObject;
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
        public void AddFish(Turn turn,Team team)
        {
            //this.fishObject[fish] = fishObject;
            if (team == Team.player)
            {
                player.Add(turn);

            }
            else
            {
                enemy.Add(turn);
            }
           
        }
        public void RemoveFish(Turn turn)
        {
            player.Remove(turn);
            enemy.Remove(turn);
            
        }
        public Turn TargetFirst(Team team)
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
        public Vector3 GetPositionOfFish(Turn turn)
        {
            if (player.Contains(turn))
            {
                print(player.IndexOf(turn));
                return  (player.IndexOf(turn) * Vector3.left * 1.5f) +playerSide.position;
            }else if (enemy.Contains(turn))
            {
                return (enemy.IndexOf(turn) * Vector3.right * 1.5f) + enemySide.position;
            }
            return Vector3.zero;
        }
    }
    [Serializable]
    public abstract class Turn
    {
        public Team team { get; protected set; }
        Team oppositeTeam { get { return team == Team.player ? Team.enemy : Team.player; } }
        public int actionsPerTurn { get; protected set; } = 1;
        public int actionsLeft { get; protected set; }

        public bool ActionLeft { get { return actionsLeft > 0 && actionsCompleted; } }
        public FishMonster fish { get; protected set; }
        public CombatDepth currentDepth { get; protected set; }
        public int depthIndex { get { return combatManager.depthIndex[currentDepth]; } }
        protected CombatManager combatManager;
        public int initiative{ get; protected set; }
        public static Action<Turn,bool> NewTurn;
        public static Action TurnEnded;
        public Action HasFeinted;
        bool actionsCompleted=true;
        HashSet<StatusEffect.StatusEffectInstance> effects = new HashSet<StatusEffect.StatusEffectInstance>();
        public float Health { get { return fish.health; } }
        public float MaxHealth { get { return fish.maxHealth; } }

        public float maxStamina { get { return fish.maxStamina; }}
        public float stamina { get { return fish.stamina; } }
        public Turn(CombatManager combatManager, FishMonster fish,CombatDepth startingDepth ,int actionsPerTurn=2)
        {
            //this.team = team;
            this.fish = fish;
            this.actionsPerTurn = actionsPerTurn;
            this.combatManager = combatManager;
            currentDepth = startingDepth;
            combatManager.CompletedAllActions += ActionsCompleted;
            fish.HasFeinted = HasFeinted;
            //stamina = maxStamina;
        }
        public virtual void StartTurn()
        {
            actionsLeft = actionsPerTurn;
            NewTurn?.Invoke(this,team==Team.player);
            TickEffects();
        }
        void ActionsCompleted()
        {
            actionsCompleted = true;
        }
        public void UseAction(int amount=1)
        {
            actionsLeft = Mathf.Clamp(actionsLeft-amount, 0, actionsPerTurn);
        }
        public void RollInitiative()
        {
            initiative = UnityEngine.Random.Range(0, 20) + fish.agility;
        }
        public void EndTurn()
        {
            fish.RecoverStamina();
            TurnEnded?.Invoke();
        }
        public void Move(int depthIndex)
        {
            actionsCompleted = false;

            CombatDepth prevDepth = currentDepth;
            CombatDepth targetDepth = combatManager.depths[depthIndex];
            if (prevDepth != null)
            {
                prevDepth.RemoveFish(this);
            }
            targetDepth.AddFish(this, team);
            currentDepth = targetDepth;
            UseAction();
            combatManager.DepthChanged(this, prevDepth, targetDepth);

        }
        public bool AbilityUsable(int abilityIndex)
        {
            Ability ability = fish.GetAbility(abilityIndex);
           
            return stamina>=ability.StaminaUsage && ability.AvailableDepths.HasFlag(currentDepth.depth);
        }
        public bool DepthTargetable(int abilityIndex,Depth depth)
        {
            
            return combatManager.depth[depth].SideHasFish(oppositeTeam)&&fish.GetAbility(abilityIndex).TargetableDepths.HasFlag(depth);
        }
        public void UseAbility(int abilityIndex,int target)
        {
            if (AbilityUsable(abilityIndex))
            {

                Ability ability = fish.GetAbility(abilityIndex);
                fish.ConsumeStamina(ability.StaminaUsage);
                actionsCompleted = false;
                //CombatDepth targetDepth = combatManager.depths[target];
                
                combatManager.UseAbility(this, abilityIndex, target);
                UseAction();
            }
            
        }
        public void ForcedMove(int direction)
        {
            int target=UnityEngine.Mathf.Clamp(depthIndex-direction, 0, 2);
            Move(target);
        }
        public void TakeDamage()
        {
            //fish.TakeDamage()
        }
        public void AddEffects(StatusEffect effect)
        {
            foreach (var e in effects)
            {
                if (e.IsEffect(effect))
                {

                    return;
                }

            }
            effects.Add(effect.NewInstance());
        }
        public void TickEffects()
        {
            HashSet<StatusEffect.StatusEffectInstance> effectsToRemove=new HashSet<StatusEffect.StatusEffectInstance>();
            foreach (StatusEffect.StatusEffectInstance effect in effects)
            {
                Debug.Log(effect);
                if (effect.DoEffect(this))
                {
                   effectsToRemove.Add(effect);
                }
               
            }
            foreach (var effect in effectsToRemove)
            {
                effects.Remove(effect);
            }
        }

    }
}



public class PlayerTurn : Turn
{
    public PlayerTurn(CombatManager combatManager, FishMonster fish, CombatDepth startingDepth, int actionsPerTurn = 2) : base(combatManager, fish, startingDepth, actionsPerTurn)
    {
        this.team = Team.player;
    }
}


public class EnemyTurn : Turn
{
    public EnemyTurn(CombatManager combatManager, FishMonster fish, CombatDepth startingDepth, int actionsPerTurn = 2) : base(combatManager, fish, startingDepth, actionsPerTurn)
    {
        this.team = Team.enemy;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        if (ActionLeft)
        {
            UseAbility(0, 0);
            EndTurn();
        }
      

    }
}