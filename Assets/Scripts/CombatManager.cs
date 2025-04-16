using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static CombatManager;
using static Element;
using static UnityEngine.VFX.VFXTypeAttribute;


public class CombatManager : MonoBehaviour,IUseDevCommands,ISaveable
{
    public enum Team
    {
        player,
        enemy
    }
    enum CombatPhase
    {
        draft,
        combat,
        postCombat
    }
    //Turn currentTurn;
    CombatPhase currentPhase=CombatPhase.draft;
    int roundNmber;

    [SerializeField]
    UIDocument ui;
    public CombatUI combatUI { get; private set; }
    public DraftUI draftUI { get; private set; }
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
    public Dictionary<Depth, CombatDepth> depth = new Dictionary<Depth, CombatDepth>();
    //Dictionary<FishMonster,CombatDepth> fishCurrentDepth=new Dictionary<FishMonster,CombatDepth>();
   public  Dictionary<CombatDepth, int> depthIndex = new Dictionary<CombatDepth, int>();

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
    [SerializeField]
    CinemachineTargetGroup targetGroup;
    public CombatAI combatAI { get; private set; }

    public object DataToSave => (enemyFishes,rewardFish,randomState);

    public string ID => "CombatManager";

    int draftedCount;

    GameObject previousSelected;
    [SerializeField]
    [HideInInspector]
    UnityEngine.Random.State randomState;

    bool hasFightEnded;
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


        //draftUI = new();
        combatUI = new CombatUI();
        ui.rootVisualElement.Add(combatUI);
        combatUI.InitialDraft();
        //ui.rootVisualElement.Add(draftUI);
        InputManager.OnInputChange += InputChanged;
        //combatUI.UseNet += UseNet;
    }
    private void OnDestroy()
    {
        InputManager.OnInputChange -= InputChanged;
        CompletedAllActions = null;
        combatUI.EnableUI(false);
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
            if (draftedCount >= 3)
            {
                return;
            }
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
        InputManager.Input.Combat.Cancel.performed += OnCancel;
        combatUI.EndDraft += CompleteDraft;
    }
    Action undoDraft;
    private void OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (combatVisualizer.CancelMove())
        {
            return;
        }

        if (currentPhase == CombatPhase.draft)
        {
            undoDraft?.Invoke();
        }

    }

    [DevConsoleCommand("KillAllEnemyFish","Use to kill all opposing fish. Only use on a player's turn to avoid bugs")]
    public static void KillAllEnemies()
    {
        CombatManager manager=FindAnyObjectByType<CombatManager>();
        enemyFishes.ForEach(f => 
        {

            manager.getFishesTurn[f].TakeDamage(10000, null, Ability.AbilityType.special);
            manager.getFishesTurn[f].CheckDeath();
           
        });
       
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
        playerFishes.ForEach((f) => f.RestoreStaminaFull());
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

    Stack<(Turn,int index)> draftStack=new();
    void DraftFish(int index,int target)
    {
        
        Turn turn = new PlayerTurn(this, playerFishes[index], depths[target % 3]);
        AddFish(turn, depths[target % 3], Team.player);
        turn.fish.RecoverStamina();
        currentCombatents.Add(turn);
        getFishesTurn[playerFishes[index]] = turn;
        draftedCount++;
        draftStack.Push((turn,index));
        undoDraft =()=> 
        {
            (Turn turn, int index) val;
            if(draftStack.TryPop(out val))
            {
                RemoveFishFromBattle(val.turn);
                combatUI.ReAddToDraft(val.index);
                draftedCount--;
            }
           
        };
        if (draftedCount >= 3 || draftedCount >= playerFishes.Count)
        {
           
            
        }

    }

    void CompleteDraft()
    {
        if (draftedCount <= 0)
        {
            return;
        }
        combatUI.EndDraft -= CompleteDraft;
        currentPhase = CombatPhase.combat;
        undoDraft = null;
        combatUI.StopDraft();
        targetGroup.m_Targets[2].weight = 0;
        OrderTurn();
        StartTurn();
        draftStack.Clear();

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
            CanFightEnd();
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
        
       
        //combatUI.SetInventory(GameManager.Instance.PlayerInventory);
        for (int i = 0; i < enemyFishes.Count; i++)
        {
            Turn turn = new EnemyTurn(this, enemyFishes[i], depths[i % 3]);
            AddFish(turn, depths[i % 3], Team.enemy);
            currentCombatents.Add(turn);
            getFishesTurn[enemyFishes[i]] = turn;
            fishCaught[enemyFishes[i]] = false;
        }

        randomState = UnityEngine.Random.state;


    }
    
    private void OnDisable()
    {
        
    }
    void AddFish(Turn turn, CombatDepth destination, Team team)
    {

        destination.AddFish(turn, team);
        combatVisualizer.AddFish(turn, destination.GetSideTransform(team).position, team);
        combatVisualizer.MoveFish(turn, destination.GetPositionOfFish(turn));
        turn.HasFeinted = () => { RemoveFishFromBattle(turn); Debug.Log(" should have removed fish"); CanFightEnd(); };

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
        if (hasFightEnded)
        {
            return;
        }
        hasFightEnded = true;
        combatUI.EnableUI(false);
        playerFishes.ForEach((f) => {if(getFishesTurn.ContainsKey(f)) f.UpdateHealth(getFishesTurn[f].Health); });
        
        StartCoroutine(CombatVictoryScreen(winningTeam));
       
    }

    IEnumerator CombatVictoryScreen(Team winningTeam)
    {
        
       
        var victoryScreen = new NewCombatVictory(playerFishes,fishCaught);
        ui.rootVisualElement.Add(victoryScreen);
        combatUI.SetEnabled(false);
        //InputManager.Input.UI.Pause.performed += Skip;
        GameManager.Instance.canPause = false;

        void Skip(InputAction.CallbackContext context)
        {

            InputManager.Input.UI.Pause.performed -= Skip;
            InputManager.OnInputChange -= InputChanged;
            GameManager.Instance.CombatEnded(winningTeam);
            GameManager.Instance.canPause = true;
            StopAllCoroutines();
        }
        
        if (winningTeam==Team.player)
        {
            var deltaXps= RewardXP();
            yield return null;
            for (float i = 0; i < 3; i+=Time.deltaTime)
            {

                foreach (var fish in playerFishes)
                {
                    var profile = victoryScreen.fishProfile[fish];
                    //victoryScreen.fishProfile[fish].value = Mathf.MoveTowards(victoryScreen.fishXpBar[fish].value, fish.Xp, 1);
                    if (profile.UpdateXpManual(deltaXps[fish].deltaXp * Time.deltaTime/3))
                    {
                        profile.UpdateLevelManual(1);
                    }


                }
                yield return null;
            }

        }
        else
        {
            yield return new WaitForSeconds(1);
            

        }
        //ui.rootVisualElement.Remove(combatUI);
        //playerFishes = null;
        //enemyFishes = null;
        //rewardFish = false;
        GameManager.Instance.canPause = true;
        InputManager.OnInputChange -= InputChanged;
        GameManager.Instance.CombatEnded(winningTeam);

    }
    Dictionary<FishMonster,(float deltaXp,int deltaLevel)> RewardXP()
    {
        Dictionary<FishMonster, (float deltaXp, int deltaLevel)> delta = new();
        foreach (FishMonster fish in playerFishes)
        {
            var deltaXp = 0f;
            int startLevel = fish.Level;
            foreach (FishMonster enemy in enemyFishes)
            {
                float xpToAdd = ((float)enemy.Level / fish.Level) * 800;
                fish.AddXp(xpToAdd);
                deltaXp += xpToAdd;
            }

            delta[fish] = (deltaXp, fish.Level - startLevel);
        }
        return delta;
    }
    void StartTurn()
    {

        
       
        
        combatVisualizer.TargetCameraToFish(currentTurn.Value);
        if (currentTurn.Value is EnemyTurn)
        {
            
        }
        currentTurn.Value.StartTurn();
        //Invoke("turnList[currentTurn].StartTurn",1);

    }
    void NextTurn()
    {
        combatVisualizer.FinishedSelecting();
        combatVisualizer.StopSelectingFish();
        //if (CanFightEnd())
        //{
        //    return;
        //}
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
            combatVisualizer.AnimateAttack(ability, turn, turn, () =>
            {
                ActionsCompleted();
                turn.CheckDeath();
                if (currentTurn.Value.team == Team.player)
                {
                    combatUI.EnableButtons();
                }
                

            });
           
            return;
        }

        if (ability.Targeting == Ability.TargetingType.all)
        {
            
            foreach (var depth in depths)
            {
                if (depth.SideHasFish(targetedTeam) && ability.DepthTargetable(depth.depth))
                {
                    if(ability.UseAbility(turn, depth))
                    {
                        combatVisualizer.AnimateAttack(ability, turn, depth.TargetFirst(targetedTeam), () =>
                        {
                            depth.TargetSide(targetedTeam).ForEach((turn) => turn.CheckDeath());
                            ActionsCompleted();

                            if (currentTurn.Value.team == Team.player)
                            {
                                combatUI.EnableButtons();
                            }

                        });
                    }
                    
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
            ability.UseAbility(turn, targetedDepth);
            combatVisualizer.AnimateAttack(ability, turn, targetedFish, () =>
            {

                targetedDepth.TargetSide(targetedTeam)?.ForEach((turn) => turn.CheckDeath());
                if (ability.ForcedMovement!=0)
                {
                    targetedFish.CheckDeath();
                }
                
                ActionsCompleted();
                if (currentTurn.Value is PlayerTurn)
                {
                    combatUI.EnableButtons();
                }
               
            });
           
        } 
    }
    void RemoveFishFromBattle(Turn turn)
    {
        combatVisualizer.RemoveFish(turn);
        
        combatUI.RemoveTurn(turn);
        turnList.Remove(turn);
        //getFishesTurn.Remove(turn.fish);
        //playerFishes.Remove(turn.fish);
        foreach (CombatDepth depth in depths)
        {
            if (depth.RemoveFish(turn,out var team))
            {
                if (team == Team.player)
                {
                    foreach (Turn t in depth.player)
                    {
                        combatVisualizer.MoveFish(t, depth.GetPositionOfFish(t));
                    }
                }
                else if (team == Team.enemy)
                {
                    foreach (Turn t in depth.enemy)
                    {
                        combatVisualizer.MoveFish(t, depth.GetPositionOfFish(t));
                    }

                }
               

            }
        


           

        }
        currentCombatents.Remove(turn);
    }

    private void Update()
    {
        if (currentTurn != null&& (combatVisualizer.turnToObject.ContainsKey(currentTurn.Value)&& combatVisualizer.turnToObject?[currentTurn.Value] != null))   
        {
            combatUI.SetTurnMarker(combatVisualizer.turnToObject[currentTurn.Value].transform);
        }
        
    }

    public void Load(string json)
    {
        var data= JsonUtility.FromJson<(List<FishMonster> enemyFishes,bool rewardFish,UnityEngine.Random.State state)>(json);
        NewCombat(data.enemyFishes, data.rewardFish);
        UnityEngine.Random.state = data.state;
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
        public bool RemoveFish(Turn turn,out Team team)
        {
            if (player.Contains(turn))
            {
                player.Remove(turn);
                team=Team.player;
                return true;
            }
            else if (enemy.Contains(turn))
            {
                enemy.Remove(turn);
                team = Team.enemy;
                return true;
            }
            team=Team.player;
            return false;

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
        float health;
        public float Health { get { return health; } }
        public float MaxHealth { get { return fish.MaxHealth; } }

        public float MaxStamina { get { return fish.MaxStamina; } }
        public float Stamina { get { return fish.Stamina; } }
        public Action<StatusEffect.StatusEffectInstance> NewEffect;
        public Action<StatusEffect.StatusEffectInstance> EffectRemoved;
        public event Action ValueChanged;
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
            health=fish.Health;
            //fish.HasFeinted =()=> { TurnEnded?.Invoke(); HasFeinted?.Invoke();Debug.Log("fish has feinted"); };
            fish.ValueChanged +=()=> ValueChanged?.Invoke();
            //stamina = maxStamina;
        }

        void TickLastEffects()
        {
            List<StatusEffect> effectsToRemove=new();
            foreach (var effect in lastEffects.Keys.ToArray())
            {
                lastEffects[effect]--;
                if (lastEffects[effect] <= 0)
                {
                    effectsToRemove.Add(effect);
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
            health-=damageOut;
            ValueChanged?.Invoke();
            return damageOut;
        }
        public void Restore(float hpToRestore)
        {
            health += hpToRestore;
            ValueChanged?.Invoke();
            combatManager.combatVisualizer.AnimateDamageNumbers(this, hpToRestore, Effectiveness.healing);
        }
        public bool CheckDeath()
        {
            if (Health <= 0)
            {
                if (actionsCompleted)
                {
                    Feint();
                }
                else
                {
                    combatManager.CompletedAllActions -= Feint;
                    combatManager.CompletedAllActions += Feint;
                }
               
                return true;
            }
            return false;
        }
        void Feint()
        {

            combatManager.CompletedAllActions -= Feint;
            //isDead = true;
            if (combatManager.currentTurn.Value == this)
            {
                EndTurn();
            }
           
            HasFeinted?.Invoke();
            Debug.Log("Should Feint or die");
        }
        public virtual void StartTurn()
        {

            actionsCompleted = true;
            actionsLeft = actionsPerTurn;
           
            
            combatManager.combatUI.NewTurn(this, team == Team.player);
            TickEffects(StatusEffect.EffectUsage.preTurn, () => 
            {
                if (team == Team.player)
                {
                    combatManager.combatUI.EnableButtons();
                }
                
                if (CheckDeath()||actionsLeft <= 0)
                {
                    //combatManager.combatUI.EnableButtons();
                    EndTurn();
                }
                
                //if (combatManager.CanFightEnd())
                //{
                //    //EndTurn();
                //    return;
                //}

            });
            


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
            if (health <= 0)
            {
                TurnEnded?.Invoke();
                combatManager.NextTurn();
                return;
            }
            if (actionsCompleted)
            {
                fish.RecoverStamina();
                TickEffects(StatusEffect.EffectUsage.postTurn, () =>
                {
                    TickLastEffects();
                    TurnEnded?.Invoke();
                    CheckDeath();
                    //combatManager.CanFightEnd();
                });
                combatManager.NextTurn();
            }
            
        }
        public void Move(Action callback)
        {
            if (ActionLeft)
            {
               
                combatManager.combatVisualizer.StartTargeting((i) => { if (i >= 0) { Move(i); combatManager.combatUI.EnableButtons(); } callback.Invoke(); });
            }
           
        }
        public void Move(int depthIndex)
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
                prevDepth.RemoveFish(this,out _);
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
            combatManager.UseItem(item, () => 
            { 
                UseAction(); 
                callback?.Invoke(true);

            }, () => 
            {
                callback?.Invoke(false);
               
            });
            
                    
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
                    combatManager.combatVisualizer.StartTargeting(DepthTargetable, abilityIndex, (i) => 
                    { 
                        if (i >= 0) 
                        { 
                            UseAbilityDirect(abilityIndex, i);
                        } 
                        callback.Invoke(); 
                    });
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
                    //NewEffect?.Invoke(instance);
                    return;
                }

            }
            var instance = effect.NewInstance(owner);
            effects.Add(instance);
            lastEffects.Remove(effect);
            instance.DurationChanged += (x) => { if (x <= 0) { RemoveEffect(instance); } };
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

        public void TickEffects(StatusEffect.EffectUsage usage,Action OnCompleted=null)
        {
            HashSet<StatusEffect.StatusEffectInstance> effectsToRemove = new HashSet<StatusEffect.StatusEffectInstance>();

            RecursiveTickEffect(usage, effects.ToArray(), 0, effects.Count, () =>
            {

                foreach (StatusEffect.StatusEffectInstance effect in effects)
                {

                    if (lastEffects.ContainsKey(effect.effect))
                    {
                        effectsToRemove.Add(effect);
                    }

                }

                foreach (var effect in effectsToRemove)
                {
                    Debug.Log(effect + " removed");
                    effects.Remove(effect);
                    EffectRemoved?.Invoke(effect);


                }
                OnCompleted?.Invoke();
            });


           
            
            
        }
        void RecursiveTickEffect(StatusEffect.EffectUsage usage,StatusEffect.StatusEffectInstance[] effects, int start, int end,Action OnComplete)
        {

            if (start >= end)
            {
                OnComplete?.Invoke();
                return;
            }

            if (usage == effects[start].effectUsage)
            {
                if (!effects[start].DoEffect(this))
                {
                    lastEffects[(effects[start].effect)] = 2;
                }
            }
            combatManager.combatVisualizer.DoStatusEffect(effects[start].effect, this, () => { RecursiveTickEffect(usage, effects, start + 1, end, OnComplete); });

        }

        void RemoveEffect(StatusEffect.StatusEffectInstance effectInstance)
        {
            if (effects.Contains(effectInstance))
            {
                lastEffects[(effectInstance.effect)] = 2;
                effectInstance.DurationChanged = null;
                effects.Remove(effectInstance);
                EffectRemoved?.Invoke(effectInstance);
            }
           
        }
        ~Turn()
        {
            fish.ValueChanged-=ValueChanged;
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
        if (actionsLeft > 0)
        {
            combatManager.combatAI.StartTurn(this);
        }
        
        //if (actionsLeft>0)
        //{
        //    Debug.Log(actionsLeft);
        //    //UseAbility(0, 0);
        //}

        //EndTurn();


    }
    


}