using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
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

    [SerializeField]
    UIDocument ui;
    public CombatUI combatUI { get; private set; }
    //CombatUI combatUI;
    //[SerializeField]
    //TurnListUI turnListUI;
    [SerializeField]
    CombatVisualizer combatVisualizer;


    [SerializeField]
    FishMonsterType testType;

    static List<FishMonster> playerFishes = new List<FishMonster>(), enemyFishes = new List<FishMonster>();
    Dictionary<FishMonster,bool> fishCaught=new();

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
    //[SerializeField]
    //CinemachineVirtualCamera virtualCamera;

    static bool rewardFish;

    public CombatAI combatAI { get; private set; }
    int draftedCount;

    GameObject previousSelected;
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
        combatAI = this.gameObject.AddComponent<CombatAI>();
        combatAI.SetCombatManager(this);
        //ui = FindObjectOfType<UIDocument>();
        combatUI = new CombatUI();
        ui.rootVisualElement.Add(combatUI);
        InputManager.OnInputChange += InputChanged;
        //combatUI.UseNet += UseNet;
    }

    private void InputChanged(InputMethod method)
    {

        if (method == InputMethod.mouseAndKeyboard)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                previousSelected = EventSystem.current.currentSelectedGameObject;
            }
           
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }
        else if(method == InputMethod.controller)
        {
            
            EventSystem.current.SetSelectedGameObject(previousSelected);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            
        }
    }

    private void Start()
    {

        SetUp();
        InputManager.DisablePlayer();
        InputManager.EnableCombat();
        InputManager.Input.UI.Enable();
        combatUI.Draft(playerFishes, (index, callback) =>
        {

            combatVisualizer.StartTargeting((target) =>
            {
                if (target < 0)
                {
                    combatUI.ResetFishPreview();
                    callback.Invoke(false);
                }
                else
                {
                    DraftFish(index, target);
                    callback.Invoke(true);
                }
                
            });
        });
        Time.timeScale = 1;
    }


    /// <summary>
    /// tells the combat manager to setup a new combat with a set of paramaters
    /// <param name="enemyFishes"></param>
    /// <param name="rewardFish"></param>
    public static void NewCombat(List<FishMonster> enemyFishes, bool rewardFish = false)
    {
        //SceneManager.sceneLoaded += OnSceneLoad;
        SceneManager.LoadSceneAsync("CombatTransition").completed+= OnSceneLoad;
       

        playerFishes = GameManager.Instance.PlayerFishventory.Fishies.ToList();
        CombatManager.enemyFishes = enemyFishes;
        CombatManager.rewardFish = rewardFish;
        
        //OrderTurn();
        //StartTurn();
    }

    private static void OnSceneLoad(AsyncOperation operation)
    {
        var battleOperation = SceneManager.LoadSceneAsync("BattleScene 1");
        battleOperation.allowSceneActivation = false;
        FindObjectOfType<PlayableDirector>().stopped += (x) => { battleOperation.allowSceneActivation=true; };
        
    }


    void DraftFish(int index,int target)
    {
        Turn turn = new PlayerTurn(this, playerFishes[index], depths[target % 3]);
        AddFish(turn, depths[target % 3], Team.player);
        turn.fish.RecoverStamina();
        currentCombatents.Add(turn);
        getFishesTurn[playerFishes[index]] = turn;
        draftedCount++;
        if (draftedCount >= 3 || draftedCount >= playerFishes.Count)
        {
            combatUI.StopDraft();
            OrderTurn();
            StartTurn();
        }
    }
    void UseItem(Item item,Action completedCallback,Action canceledCallback)
    {
        
        if(item is CombatHook)
        {
            combatVisualizer.SelectFish(Team.enemy,(t) => 
            {
                TryCatching((CombatHook)item, t, completedCallback);

            },canceledCallback);
        }
        else if (item is Potion)
        {
            UsePotion((Potion)item, currentTurn.Value);
            ActionsCompleted();
            combatUI.UpdateInventory();
            combatUI.EnableButtons();
            completedCallback?.Invoke();
        }
        
       
    }
    void TryCatching(CombatHook hook, Turn target, Action completedCallback)
    {
        if (!rewardFish)
        {
            return;
        }
        bool caught = UnityEngine.Random.Range(0, 1f) <= (1 - (target.Health / target.MaxHealth)) * hook.CatchBonus;
        combatVisualizer.CatchFishAnimation(target, caught, () =>
        {
            if (caught)
            {
                CatchFish(target);
            }
            ActionsCompleted();
            completedCallback?.Invoke();
            currentTurn.Value.UseAction();
            GameManager.Instance.PlayerInventory.RemoveItem(hook);
            combatUI.UpdateInventory();
        });

        
    }
    void CatchFish(Turn target)
    {
        target.fish.RestoreAllHealth();
        GameManager.Instance.CapturedFish(target.fish);
        fishCaught[target.fish] = true;
        RemoveFishFromBattle(target);
        Debug.Log("caught " + target.fish);
    }


    void UsePotion(Potion potion,Turn target)
    {
        potion.UsePotion((PlayerTurn)target, (particle) => combatVisualizer.AnimateBasicVFX(target, particle));
        GameManager.Instance.PlayerInventory.RemoveItem(potion);
        //currentTurn.Value.UseAction();

    }
    //sets up the combat
    void SetUp()
    {
        if (InputManager.inputMethod== InputMethod.mouseAndKeyboard)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
        
        Turn.TurnEnded += NextTurn;
        combatUI.SetInventory(GameManager.Instance.PlayerInventory);
        for (int i = 0; i < enemyFishes.Count; i++)
        {
            Turn turn = new EnemyTurn(this, enemyFishes[i], depths[i % 3]);
            AddFish(turn, depths[i % 3], Team.enemy);
            currentCombatents.Add(turn);
            getFishesTurn[enemyFishes[i]] = turn;
            fishCaught[enemyFishes[i]] = false;
        }
        
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
        foreach(Turn turn in currentCombatents.OrderBy((x) => x.initiative))
        {
            turnList.AddLast(turn);
        }
        currentTurn = turnList.First;
        combatUI.SetTurnUI(turnList.ToList());
        
        //turnListUI.SetTurnBar(turnList.ToList());
    }
    void ActionsCompleted()
    {
        actionsCompleted = true;

        
        CompletedAllActions?.Invoke();
        CanFightEnd();

    }
    bool CanFightEnd()
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
            return true;
        }
        else if (numOfFriendly <= 0)
        {
            EndFight(Team.enemy);
            return true;
        }
        return false;
    }
    void EndFight(Team winningTeam)
    {
       
        StartCoroutine(CombatVictoryScreen(winningTeam));
 
    }

    IEnumerator CombatVictoryScreen(Team winningTeam)
    {
        var victoryScreen = new CombatVictory(playerFishes,enemyFishes,fishCaught);
        ui.rootVisualElement.Add(victoryScreen);
        combatUI.SetEnabled(false);
       
        if (winningTeam==Team.player)
        {

            foreach (var fish in playerFishes)
            {
                victoryScreen.fishXpBar[fish].value = fish.Xp;
            }
            RewardXP();
            yield return new WaitForFixedUpdate();
            for (int i = 0; i < 300; i++)
            {

                foreach (var fish in playerFishes)
                {
                    victoryScreen.fishXpBar[fish].value = Mathf.MoveTowards(victoryScreen.fishXpBar[fish].value, fish.Xp, 1);
                   


                }
                yield return new WaitForFixedUpdate();
            }

        }
        else
        {
            for (int i = 0; i < 300; i++)
            {
                yield return new WaitForFixedUpdate();
            }

        }
        //ui.rootVisualElement.Remove(combatUI);
        //playerFishes = null;
        //enemyFishes = null;
        //rewardFish = false;
        InputManager.OnInputChange -= InputChanged;
        GameManager.Instance.CombatEnded(winningTeam);

    }
    void RewardXP()
    {
        foreach(FishMonster fish in playerFishes)
        {
            foreach(FishMonster enemy in enemyFishes)
            {
                fish.AddXp((enemy.Level / fish.Level)*100);
            }
            
        }
    }
    void StartTurn()
    {

        
        currentTurn.Value.StartTurn();
        
        combatVisualizer.TargetCameraToFish(currentTurn.Value);
        if (currentTurn.Value is EnemyTurn)
        {
            
        }
        //Invoke("turnList[currentTurn].StartTurn",1);

    }
    void NextTurn()
    {
        if (CanFightEnd())
        {
            return;
        }
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
        actionsCompleted = false;
        if (ability.TargetedTeam == Ability.TargetTeam.friendly)
        {
            targetedTeam = turn.team;
        }
        else if (ability.TargetedTeam == Ability.TargetTeam.enemy)
        {
            targetedTeam = turn.team == Team.player ? Team.enemy : Team.player;
        }else if (ability.TargetedTeam == Ability.TargetTeam.self)
        {
            bool hit;
            float damageDone;
            ability.UseAbility(turn, turn, out hit, out damageDone);
            ActionsCompleted();
            combatUI.EnableButtons();
            turn.fish.CheckDeath();
            return;
        }

        if (ability.Targeting == Ability.TargetingType.all)
        {
            List<Turn> targets = new();
            if (ability.Piercing)
            {
                targets.AddRange(depth[Depth.shallow].TargetSide(targetedTeam));
                targets.AddRange(depth[Depth.middle].TargetSide(targetedTeam));
                targets.AddRange(depth[Depth.abyss].TargetSide(targetedTeam));
                
            }
            else
            {
                targets.Add(depth[Depth.shallow].TargetFirst(targetedTeam));
                targets.Add(depth[Depth.middle].TargetFirst(targetedTeam));
                targets.Add(depth[Depth.abyss].TargetFirst(targetedTeam));
            }
            
           
            targets[1] = depth[Depth.middle].TargetFirst(targetedTeam);
            targets[2] = depth[Depth.abyss].TargetFirst(targetedTeam);

            foreach (var target in targets)
            {
                if (target != null)
                {
                    bool hit;
                    float damageDone;
                    
                    ability.UseAbility(turn, target, out hit,out damageDone);
                    
                    combatVisualizer.AnimateAttack(ability,turn, target, () => 
                    {  
                        if (hit)
                        {
                           
                            if (ability.ForcedMovement != 0)
                            {
                                target.ForcedMove(ability.ForcedMovement);
                            }
                            
                        }
                        ActionsCompleted();
                        combatUI.EnableButtons();
                        target.fish.CheckDeath();
                    });
                }
            }
        }
        else if (ability.Targeting == Ability.TargetingType.single)
        {
            CombatDepth targetedDepth = depths[depthIndex];

            List<Turn> targetedFish = ability.Piercing? targetedDepth.TargetSide(targetedTeam) : new(){targetedDepth.TargetFirst(targetedTeam)} ;
            if (targetedFish == null)
            {
                ActionsCompleted();
                return;
            }
            //ui.UpdateActionsLeft(turnList[currentTurn].actionsLeft);
            // var attackingFish = turn.fish;
           
            foreach (var target in targetedFish)
            {
                bool hit;
                float damageDone;
                
                ability.UseAbility(turn, target, out hit, out damageDone);
                combatVisualizer.AnimateAttack(ability, turn, target, () => 
                {
                    if (hit)
                    {
                       
                        if (ability.ForcedMovement != 0)
                        {
                            target.ForcedMove(ability.ForcedMovement);
                        }
                    }
                    
                    target.fish.CheckDeath();
                    ActionsCompleted();
                    combatUI.EnableButtons();
                });
            }
           
        } 
    }
    void RemoveFishFromBattle(Turn turn)
    {
        combatUI.RemoveTurn(turn);
        turnList.Remove(turn);
        //playerFishes.Remove(turn.fish);
        foreach (CombatDepth depth in depths)
        {
            depth.RemoveFish(turn);
        }
        combatVisualizer.RemoveFish(turn);
    }

    private void Update()
    {
        if (currentTurn != null&& (combatVisualizer.turnToObject.ContainsKey(currentTurn.Value)|| combatVisualizer.turnToObject?[currentTurn.Value] != null))
        {
            combatUI.SetTurnMarker(combatVisualizer.turnToObject[currentTurn.Value].transform);
        }
        
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
        public bool HasFish(Turn turn)
        {
            return player.Contains(turn)||enemy.Contains(turn);
        }
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
        public List<Turn> TargetSide(Team team)
        {
            if (team == Team.player && player.Count > 0)
            {
                return player.ToList();
            }
            else if (team == Team.enemy && enemy.Count > 0)
            {
                return enemy.ToList();
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
        public bool IsCurrentTurn { get { return combatManager.currentTurn.Value == this; } }
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
        public Dictionary<StatusEffect,int> lastEffects { get; private set; } = new();

        public float Health { get { return fish.Health; } }
        public float MaxHealth { get { return fish.MaxHealth; } }

        public float MaxStamina { get { return fish.MaxStamina; } }
        public float Stamina { get { return fish.Stamina; } }
        public Action<StatusEffect.StatusEffectInstance> NewEffect;
        public Action<StatusEffect.StatusEffectInstance> EffectRemoved;
        public int agility
        {
            get
            {
                return Mathf.RoundToInt(fish.Agility.value + (fish.Agility.value * GetAttributeMulti("aglity")) + GetAttributeMod("agility"));
            }
        }
        public float dodge { get { return (fish.Dodge + (fish.Dodge * GetAttributeMulti("dodge")) + GetAttributeMod("dodge")); } }
        public int accuracy
        {
            get
            {
                return Mathf.RoundToInt(fish.Accuracy.value + (fish.Accuracy.value * GetAttributeMulti("accuracy")) + GetAttributeMod("accuracy"));
            }
        }
        public int attack
        {
            get
            {
                
                return Mathf.RoundToInt(fish.Attack.value + (fish.Attack.value * GetAttributeMulti("attack")) + GetAttributeMod("attack"));
            }
        }
        public int special
        {
            get
            {
                return Mathf.RoundToInt(fish.Special.value + (fish.Special.value * GetAttributeMulti("special")) + GetAttributeMod("special"));
            }
        }
        public int fortitude
        {
            get
            {
                return Mathf.RoundToInt(fish.Fortitude.value + (fish.Fortitude.value * GetAttributeMulti("fortitude")) + GetAttributeMod("fortitude"));
            }
        }
        public int specialFort
        {
            get
            {
                return Mathf.RoundToInt(fish.SpecialFort.value + (fish.SpecialFort.value * GetAttributeMulti("specialFort")) + GetAttributeMod("specialFort"));
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
        float GetAttributeMulti(string name)
        {
            float val = 0;
            foreach (StatusEffect effect in effects.Select(x => x.effect))
            {
                if (effect is StatMultiplierStatusEffect)
                {
                    val += (effect as StatMultiplierStatusEffect).Attribute[name];
                }
                else if (effect is CompoundEffect)
                {
                    CompoundEffect compoundEffect = (CompoundEffect)effect;
                    foreach (var e in compoundEffect.Effects)
                    {
                        val += (e as StatMultiplierStatusEffect).Attribute[name];
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

        void TickLastEffects()
        {
            List<StatusEffect> effectsToRemove=new();
            foreach (var effect in lastEffects)
            {
                lastEffects[effect.Key]--;
                if (effect.Value <= 0)
                {
                    effectsToRemove.Add(effect.Key);
                }
            }

            foreach(var effect in effectsToRemove)
            {
                lastEffects.Remove(effect);
            }
        }

        public float TakeDamage(float damage, Element elementType, Ability.AbilityType abilityType )
        {
            Element.Effectiveness effectiveness;
            var damageOut= fish.TakeDamage(damage, elementType, abilityType, out effectiveness);
            combatManager.combatVisualizer.AnimateDamageNumbers(this, damageOut, effectiveness);
            return damageOut;
        }
        public virtual void StartTurn()
        {
            
           
            actionsLeft = actionsPerTurn;
            TickEffects(StatusEffect.EffectUsage.preTurn);
            combatManager.combatUI.NewTurn(this, team == Team.player);
            
            if (combatManager.CanFightEnd())
            {
                EndTurn();
            }
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
            
            if (actionsCompleted)
            {
                fish.RecoverStamina();
                TickEffects(StatusEffect.EffectUsage.postTurn);
                TurnEnded?.Invoke();
                combatManager.CanFightEnd();
            }
            
        }
        public void Move(Action callback)
        {
            if (ActionLeft)
            {
               
                combatManager.combatVisualizer.StartTargeting((i) => { if (i >= 0) { Move(i); combatManager.combatUI.EnableButtons(); } callback.Invoke(); });
            }
           
        }
        void Move(int depthIndex)
        {
            if(depthIndex < 0)
            {
                return;
            }
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

            return combatManager.depth[depth].SideHasFish(fish.GetAbility(abilityIndex).TargetedTeam ==Ability.TargetTeam.enemy? oppositeTeam:team) 
                && fish.GetAbility(abilityIndex).TargetableDepths.HasFlag(depth)
                && !(fish.GetAbility(abilityIndex).ignoreSelf && currentDepth.TargetFirst(team)==this &&currentDepth.depth==depth);
        }

        public bool ItemUsable(Item item)
        {
            if(item is CombatHook&&rewardFish==false)
            {
                return false;
            }

            return ActionLeft && GameManager.Instance.PlayerInventory.Contains(item);
        }
        public void UseItem(Item item,Action<bool> callback)
        {
            if (!ActionLeft)
            {
                return;
            }
            combatManager.UseItem(item, () => { UseAction(); callback?.Invoke(true); }, () => {callback?.Invoke(false);});
            
                    
        }
        public void UseAbility(int abilityIndex,Action callback=null)
        {
            if (AbilityUsable(abilityIndex))
            {
                //Ability ability = fish.GetAbility(abilityIndex);
                if(fish.GetAbility(abilityIndex).TargetedTeam == Ability.TargetTeam.self)
                {
                    UseAbilityDirect(abilityIndex, depthIndex);
                    callback.Invoke();
                }
                else
                {
                    combatManager.combatVisualizer.StartTargeting(DepthTargetable, abilityIndex, (i) => { if (i >= 0) { UseAbilityDirect(abilityIndex, i); } callback.Invoke(); });
                }
               

            }

        }
        public void UseAbilityDirect(int abilityIndex, int depthIndex)
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
      
        public void AddEffects(StatusEffect effect, CombatManager.Turn owner)
        {
            foreach (var e in effects)
            {
                if (e.IsEffect(effect))
                {
                    e.ResetEffect(owner);
                    return;
                }

            }
            var instance = effect.NewInstance(owner);
            effects.Add(instance);
            NewEffect?.Invoke(instance);
        }
        public bool HadEffectLastTurn(StatusEffect effect)
        {
            var b=lastEffects.ContainsKey(effect);
            if (b)
            {
                Debug.Log($"had {effect} last turns");
            }
            return b;
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
                        lastEffects[(effect.effect)]=2;
                    }
                }
                    

            }
            foreach (var effect in effectsToRemove)
            {
                Debug.Log(effect + " removed");
                effects.Remove(effect);
                EffectRemoved?.Invoke(effect);
                

            }
            fish.CheckDeath();
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