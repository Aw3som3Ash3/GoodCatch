using Cinemachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static CombatManager;

public class CombatManager : MonoBehaviour
{
    public enum Team
    {
        player,
        enemy
    }
    //Turn currentTurn;
   
    int roundNmber;

    //[SerializeField]
    UIDocument ui;
    public CombatUI combatUI { get; private set; }
    //CombatUI combatUI;
    //[SerializeField]
    //TurnListUI turnListUI;
    [SerializeField]
    CombatVisualizer combatVisualizer;


    [SerializeField]
    FishMonsterType testType;

    List<FishMonster> playerFishes = new List<FishMonster>(), enemyFishes = new List<FishMonster>();

    [SerializeField]
    public CombatDepth[] depths { get; private set; } = new CombatDepth[3];
    Dictionary<Depth, CombatDepth> depth = new Dictionary<Depth, CombatDepth>();
    //Dictionary<FishMonster,CombatDepth> fishCurrentDepth=new Dictionary<FishMonster,CombatDepth>();
    Dictionary<CombatDepth, int> depthIndex = new Dictionary<CombatDepth, int>();

    [SerializeField]
    Transform shallowsLocation, middleLocation, abyssLocation;


    HashSet<Turn> currentCombatents = new HashSet<Turn>();
    LinkedList<Turn> turnList = new LinkedList<Turn>();
    LinkedListNode<Turn> currentTurn;

    Dictionary<FishMonster, Turn> getFishesTurn = new Dictionary<FishMonster, Turn>();

    bool actionsCompleted;
    public Action CompletedAllActions;
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    Camera cam;

    Camera prevCam;
    bool rewardFish;

    public CombatAI combatAI { get; private set; }
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
        combatAI = this.AddComponent<CombatAI>();
        combatAI.SetCombatManager(this);
        ui = FindObjectOfType<UIDocument>();
        combatUI = new CombatUI();
        ui.rootVisualElement.Add(combatUI);
        //combatUI.UseNet += UseNet;

    }
    private void Start()
    {



    }
    /// <summary>
    /// tells the combat manager to setup a new combat with a set of paramaters
    /// <param name="playerFishes"></param>
    /// <param name="enemyFishes"></param>
    /// <param name="rewardFish"></param>
    public void NewCombat(List<FishMonster> playerFishes, List<FishMonster> enemyFishes, bool rewardFish = false)
    {
        this.playerFishes = playerFishes;
        this.enemyFishes = enemyFishes;
        this.rewardFish = rewardFish;
        SetUp();
        StartTurn();
        InputManager.DisablePlayer();
        InputManager.Input.UI.Enable();
    }
    void UseItem(Item item)
    {
        if(item is Net)
        {
            combatVisualizer.SelectFish(Team.enemy,(t) => 
            { 
                TryCatching((Net)item,t); 
                ActionsCompleted(); 
                combatUI.UpdateInventory();
            });
        }
        else if (item is Potion)
        {
            combatVisualizer.SelectFish(Team.player, (t) => 
            {
                UsePotion((Potion)item, t);
                ActionsCompleted();
                combatUI.UpdateInventory();
            });
        }
        GameManager.Instance.PlayerInventory.RemoveItem(item);

    }
    void TryCatching(Net net,Turn target)
    {
        if (UnityEngine.Random.Range(0, 1f) <= (1 - (target.Health / target.MaxHealth))*net.CatchBonus)
        {

            target.fish.RestoreAllHealth();
            GameManager.Instance.CapturedFish(target.fish);
            RemoveFishFromBattle(target);
            Debug.Log("caught "+ target.fish);
        }
        else
        {
            Debug.Log("failed to catch");
            
        }
        currentTurn.Value.UseAction();
    }
    void UsePotion(Potion potion,Turn target)
    {
        potion.UsePotion((PlayerTurn)target, (particle) => combatVisualizer.AnimateBasicVFX(target, particle));
    }
    //sets up the combat
    void SetUp()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        prevCam = Camera.main;
        prevCam.gameObject.SetActive(false);
        cam.enabled = true;
        Camera.SetupCurrent(cam);
        Turn.TurnEnded += NextTurn;
        combatUI.SetInventory(GameManager.Instance.PlayerInventory);
        for (int i = 0; i < playerFishes.Count; i++)
        {
            Turn turn = new PlayerTurn(this, playerFishes[i], depths[i % 3]);
            AddFish(turn, depths[i % 3], Team.player);
            currentCombatents.Add(turn);
            getFishesTurn[playerFishes[i]] = turn;

        }
        for (int i = 0; i < enemyFishes.Count; i++)
        {
            Turn turn = new EnemyTurn(this, enemyFishes[i], depths[i % 3]);
            AddFish(turn, depths[i % 3], Team.enemy);
            currentCombatents.Add(turn);
            getFishesTurn[enemyFishes[i]] = turn;
        }


        OrderTurn();

    }
    private void OnDisable()
    {
        Turn.TurnEnded -= NextTurn;
    }
    void AddFish(Turn turn, CombatDepth destination, Team team)
    {

        destination.AddFish(turn, team);
        combatVisualizer.AddFish(turn, destination.GetSideTransform(team).position, team);
        combatVisualizer.MoveFish(turn, destination.GetPositionOfFish(turn));
        turn.HasFeinted = () => { RemoveFishFromBattle(turn); Debug.Log(" should have removed fish"); };

    }
    //orders the turns by speed value
    void OrderTurn()
    {
        foreach (var turn in currentCombatents)
        {
            turn.RollInitiative();
        }
        turnList.Clear();
        turnList.AddRange(currentCombatents.OrderBy((x)=>x.initiative));
        currentTurn = turnList.First;
        combatUI.SetTurnUI(turnList.ToList());
        
        //turnListUI.SetTurnBar(turnList.ToList());
    }
    void ActionsCompleted()
    {
        actionsCompleted = true;

        CanFightEnd();
        CompletedAllActions?.Invoke();
    }
    void CanFightEnd()
    {
        int numOfFriendly = 0;
        int numOfEnemy = 0;
        foreach (Turn turn in turnList)
        {
            if (turn.team == Team.player)
            {
                numOfFriendly++;
            }
            else if (turn.team == Team.enemy)
            {
                numOfEnemy++;
            }
        }
        if (numOfEnemy <= 0)
        {
            EndFight(Team.player);
        }
        else if (numOfFriendly <= 0)
        {
            EndFight(Team.enemy);
        }
    }
    void EndFight(Team winningTeam)
    {

        if (winningTeam == Team.player)
        {
            RewardXP();
        }else if (winningTeam == Team.enemy)
        {
            GameManager.Instance.PlayerLost();
        }
        prevCam.gameObject.SetActive(true);
        Camera.SetupCurrent(prevCam);
        ui.rootVisualElement.Remove(combatUI);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("BattleScene 1"));
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        InputManager.EnablePlayer();
        InputManager.Input.UI.Disable();
    }
    void RewardXP()
    {
        foreach(FishMonster fish in playerFishes)
        {
            foreach(FishMonster enemy in enemyFishes)
            {
                fish.AddXp((enemy.level / fish.level)*100);
            }
            
        }
    }
    void StartTurn()
    {
        
        currentTurn.Value.StartTurn();
        CanFightEnd();
        if (currentTurn.Value is EnemyTurn)
        {
            
        }
        //Invoke("turnList[currentTurn].StartTurn",1);

    }
    void NextTurn()
    {
        if (!actionsCompleted && currentTurn.Value.team == Team.player)
        {
            return;
        }
        if (currentTurn.Next != null)
        {
            currentTurn = currentTurn.Next;
        }else
        {
            //OrderTurn();
            currentTurn = turnList.First;
            roundNmber++;
        }
        combatUI.NextTurn();
        //selectedFish.RecoverStamina();
        StartTurn();
    }

    void DepthChanged(Turn turn, CombatDepth previous, CombatDepth destination)
    {

        combatVisualizer.MoveFish(turn, destination.GetPositionOfFish(turn), ActionsCompleted);
        if (previous != null)
        {
            foreach (Turn t in previous.player)
            {
                combatVisualizer.MoveFish(t, previous.GetPositionOfFish(t));
            }
            foreach (Turn t in previous.enemy)
            {
                combatVisualizer.MoveFish(t, previous.GetPositionOfFish(t));
            }
        }

    }
    void UseAbility(Turn turn, Ability ability, int depthIndex)
    {
        //Ability ability = turn.fish.GetAbility(index);
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
        if (ability.Targeting == Ability.TargetingType.all)
        {
            targets[0] = depth[Depth.shallow].TargetFirst(targetedTeam);
            targets[1] = depth[Depth.middle].TargetFirst(targetedTeam);
            targets[2] = depth[Depth.abyss].TargetFirst(targetedTeam);

            foreach (var target in targets)
            {
                if (target != null)
                {
                    bool hit;
                    ability.UseAbility(turn, target, out hit);
                    combatVisualizer.AnimateAttack(ability,turn, target, () => { ActionsCompleted(); if (ability.ForcedMovement != 0) { target.ForcedMove(ability.ForcedMovement);  } combatUI.EnableButtons(); });
                }
            }
        }
        else if (ability.Targeting == Ability.TargetingType.single)
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
            ability.UseAbility(turn, targetedFish, out hit);
            combatVisualizer.AnimateAttack(ability,turn, targetedFish, () => { ActionsCompleted(); if (ability.ForcedMovement != 0) { targetedFish.ForcedMove(ability.ForcedMovement);  } combatUI.EnableButtons(); });
        }
    }
    void RemoveFishFromBattle(Turn turn)
    {
        combatUI.RemoveTurn(turn);
        turnList.Remove(turn);
        //turnListUI.RemoveTurn(turn);
        //enemyFishes.Remove(turn.fish);
        playerFishes.Remove(turn.fish);
        foreach (CombatDepth depth in depths)
        {
            depth.RemoveFish(turn);
        }
        combatVisualizer.RemoveFish(turn);
    }

    private void Update()
    {
        combatUI.SetTurnMarker(combatVisualizer.turnToObject[currentTurn.Value].transform);
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
        public void AddFish(Turn turn, Team team)
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
            if (team == Team.player && player.Count > 0)
            {
                return player[0];
            }
            else if (team == Team.enemy && enemy.Count > 0)
            {
                return enemy[0];
            }
            else
            {
                return null;
            }

        }
        public void SwapFish(int index, Team team)
        {
            if (team == Team.player)
            {
                player.Move(index, 0);
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
                return (player.IndexOf(turn) * Vector3.left * 3f) + playerSide.position;
            }
            else if (enemy.Contains(turn))
            {
                return (enemy.IndexOf(turn) * Vector3.right * 3f) + enemySide.position;
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

        public bool ActionLeft { get { return actionsLeft > 0; } }
        public FishMonster fish { get; protected set; }
        public CombatDepth currentDepth { get; protected set; }
        public int depthIndex { get { return combatManager.depthIndex[currentDepth]; } }
        protected CombatManager combatManager;
        public int initiative { get; protected set; }
        public static Action<Turn, bool> NewTurn;
        public static Action TurnEnded;
        public Action HasFeinted;
        bool actionsCompleted = true;
        public HashSet<StatusEffect.StatusEffectInstance> effects { get; private set; } = new HashSet<StatusEffect.StatusEffectInstance>();

        public float Health { get { return fish.health; } }
        public float MaxHealth { get { return fish.maxHealth; } }

        public float MaxStamina { get { return fish.maxStamina; } }
        public float Stamina { get { return fish.stamina; } }
        public Action<StatusEffect.StatusEffectInstance> NewEffect;
        public Action<StatusEffect.StatusEffectInstance> EffectRemoved;
        public int agility
        {
            get
            {
                return fish.Agility.value + GetAttributeMod("agility");
            }
        }
        public int dodge { get { return agility / 2; } }
        public int accuracy
        {
            get
            {
                return fish.Accuracy.value + GetAttributeMod("accuracy");
            }
        }
        public int attack
        {
            get
            {
                
                return fish.Attack.value + GetAttributeMod("attack");
            }
        }
        public int special
        {
            get
            {
                return fish.Special.value + GetAttributeMod("special");
            }
        }
        public int fortitude
        {
            get
            {
                return fish.Fortitude.value + GetAttributeMod("fortitude");
            }
        }
        public int specialFort
        {
            get
            {
                return fish.SpecialFort.value + GetAttributeMod("specialFort");
            }
        }
        int GetAttributeMod(string name)
        {
            int val = 0;
            foreach (StatusEffect effect in effects.Select(x => x.effect))
            {
                if (effect is StatModifierStatusEffect)
                {
                    val += (effect as StatModifierStatusEffect).Attribute[name];
                }
                else if (effect is CompoundEffect)
                {
                    CompoundEffect compoundEffect = (CompoundEffect)effect;
                    foreach (var e in compoundEffect.Effects)
                    {
                        val += (e as StatModifierStatusEffect).Attribute[name];
                    }
                }


            }
            return val;
        }
        public Turn(CombatManager combatManager, FishMonster fish, CombatDepth startingDepth, int actionsPerTurn = 1)
        {
            //this.team = team;
            this.fish = fish;
            this.actionsPerTurn = actionsPerTurn;
            this.combatManager = combatManager;
            currentDepth = startingDepth;
            combatManager.CompletedAllActions += ActionsCompleted;
            fish.HasFeinted =()=> { HasFeinted?.Invoke(); Debug.Log("fish has feinted"); };
            //stamina = maxStamina;
        }
        public virtual void StartTurn()
        {
            actionsLeft = actionsPerTurn;
            TickEffects(StatusEffect.EffectUsage.preTurn);
            combatManager.combatUI.NewTurn(this, team == Team.player);
            //NewTurn?.Invoke(this, team == Team.player);

            
        }
        void ActionsCompleted()
        {
            actionsCompleted = true;
        }
        public void UseAction(int amount = 1)
        {
            actionsLeft = Mathf.Clamp(actionsLeft - amount, 0, actionsPerTurn);
            Debug.Log("remaining actions: " + actionsLeft);
        }
        public void RollInitiative()
        {
            initiative = UnityEngine.Random.Range(0, 20) + fish.Agility.value;
        }
        public void EndTurn()
        {
            fish.RecoverStamina();
            TickEffects(StatusEffect.EffectUsage.postTurn);
            TurnEnded?.Invoke();
        }
        public void Move()
        {
            if (ActionLeft)
            {
               
                combatManager.combatVisualizer.StartTargeting((i) => { Move(i); combatManager.combatUI.EnableButtons(); });
            }
           
        }
        void Move(int depthIndex)
        {
            actionsCompleted = false;
            CombatDepth prevDepth = currentDepth;
            CombatDepth targetDepth = combatManager.depths[depthIndex];
            if (prevDepth == targetDepth)
            {
                return;
            }
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

            return Stamina >= ability.StaminaUsage && ability.AvailableDepths.HasFlag(currentDepth.depth)&&ActionLeft;
        }
        public bool DepthTargetable(int abilityIndex, Depth depth)
        {

            return combatManager.depth[depth].SideHasFish(fish.GetAbility(abilityIndex).TargetedTeam ==Ability.TargetTeam.enemy? oppositeTeam:team) && fish.GetAbility(abilityIndex).TargetableDepths.HasFlag(depth);
        }
        public void UseItem(Item item)
        {
            combatManager.UseItem(item);
            UseAction();
        }
        public void UseAbility(int abilityIndex)
        {
            if (AbilityUsable(abilityIndex))
            {
                //Ability ability = fish.GetAbility(abilityIndex);

                combatManager.combatVisualizer.StartTargeting(DepthTargetable,abilityIndex ,(i) => { UseAbility(abilityIndex,i); });
            }

        }
        public void UseAbility(int abilityIndex, int depthIndex)
        {
            if (AbilityUsable(abilityIndex))
            {

                Ability ability = fish.GetAbility(abilityIndex);
                fish.ConsumeStamina(ability.StaminaUsage);
                actionsCompleted = false;

                combatManager.UseAbility(this, ability, depthIndex);
                UseAction();
            }
        }
        public void ForcedMove(int direction)
        {
            int target = UnityEngine.Mathf.Clamp(depthIndex - direction, 0, 2);
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
            var instance = effect.NewInstance();
            effects.Add(instance);
            NewEffect?.Invoke(instance);
        }
        public void TickEffects(StatusEffect.EffectUsage usage)
        {
            HashSet<StatusEffect.StatusEffectInstance> effectsToRemove = new HashSet<StatusEffect.StatusEffectInstance>();
            foreach (StatusEffect.StatusEffectInstance effect in effects)
            {
                Debug.Log(effect);
                if(usage == effect.effectUsage)
                {
                    if (!effect.DoEffect(this))
                    {
                        effectsToRemove.Add(effect);
                    }
                }
                    

            }
            foreach (var effect in effectsToRemove)
            {
                Debug.Log(effect + " removed");
                effects.Remove(effect);
                EffectRemoved?.Invoke(effect);
            }
        }


    }
}



public class PlayerTurn : Turn
{
    public PlayerTurn(CombatManager combatManager, FishMonster fish, CombatDepth startingDepth, int actionsPerTurn = 1) : base(combatManager, fish, startingDepth, actionsPerTurn)
    {
        this.team = Team.player;
    }
}


public class EnemyTurn : CombatManager.Turn
{
    public EnemyTurn(CombatManager combatManager, FishMonster fish, CombatDepth startingDepth, int actionsPerTurn =1 ) : base(combatManager, fish, startingDepth, actionsPerTurn)
    {
        this.team = Team.enemy;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        combatManager.combatAI.StartTurn(this);
        //if (actionsLeft>0)
        //{
        //    Debug.Log(actionsLeft);
        //    //UseAbility(0, 0);
        //}

        //EndTurn();


    }
    


}