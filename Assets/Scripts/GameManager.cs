
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static CombatManager;

public class GameManager : MonoBehaviour,ISaveable,IUseDevCommands
{
    public static GameManager Instance;
    [SerializeField]
    FishDatabase database;
    public FishDatabase Database { get { return database; } }

    [Serializable]
    struct GameData
    {
        public Fishventory PlayerFishventory;
        public Fishventory StoredFishventory;
        public ItemInventory PlayerInventory;
        public float dayTime;
        public bool[] hasSeenFish;
        public string currentInnId;
        public int currentScene;
        public GameData(int partySize)
        {
            PlayerFishventory = new Fishventory(partySize);
            PlayerInventory = new ItemInventory();
            StoredFishventory = new Fishventory();
            dayTime =0;
            hasSeenFish = new bool[0];
            currentInnId = "";
            currentScene = 0;

        }
    }
    public List<bool> HasSeenFish{ get { return gameData.hasSeenFish.ToList(); } }
    GameData gameData=new GameData(6);
    public Fishventory PlayerFishventory { get { return gameData.PlayerFishventory; }}
    public Fishventory StoredFishventory { get { return gameData.StoredFishventory; }}
    public ItemInventory PlayerInventory { get { return gameData.PlayerInventory; } }
    public float DayTime { get { return gameData.dayTime; } private set { gameData.dayTime = value; } }
    public int Day { get; private set; }
    [SerializeField]
    [Range(0,24)]
    float startingTime;
    [SerializeField]
    [Min(1)]
    float secondsPerHour = 1;
    GameObject sun;
    [SerializeField]
    FishMonsterType[] testfisth;
    [SerializeField]
    Item[] startingItems;
    [SerializeField]
    TextMeshProUGUI tempTimeOfDayText;

    [SerializeField]
    VolumeProfile postProcessing;

    [SerializeField]
    public Inn lastInnVisited { get { return Inn.innIds[gameData.currentInnId]; } }
    event Action OnPlayerLost;
    bool inCombat;
    [Flags]
    public enum TimeOfDay
    {
        Day = 1,
        Night = 2,
        EarlyMorning = 6,
        Dawn = 9,
        Morning = 17,
        Noon = 33,
        Afternoon = 65,
        Evening = 129,
        Dusk = 258,
        LateNight = 514,
        Midnight = 1026,



    }
    public TimeOfDay CurrentTimeOfDay => GetTimeOfDay(DayTime);
    public TimeOfDay GetTimeOfDay(float time)
    {
        if (DayTime >= 3 && DayTime < 6)
        {
            return TimeOfDay.EarlyMorning;
        }
        else if (DayTime >= 6 && DayTime < 9)
        {
            return TimeOfDay.Dawn;
        }
        else if (DayTime >= 9 && DayTime < 12)
        {
            return TimeOfDay.Morning;
        }
        else if (DayTime >= 9 && DayTime < 12)
        {
            return TimeOfDay.Morning;
        }
        else if (DayTime >= 12 && DayTime < 13)
        {
            return TimeOfDay.Noon;
        }
        else if (DayTime >= 13 && DayTime < 15)
        {
            return TimeOfDay.Afternoon;
        }
        else if (DayTime >= 15 && DayTime < 18)
        {
            return TimeOfDay.Evening;
        }
        else if (DayTime >= 18 && DayTime < 21)
        {
            return TimeOfDay.Dusk;
        }
        else if (DayTime >= 21 && DayTime < 24)
        {
            return TimeOfDay.LateNight;
        }
        else if (DayTime >= 0 && DayTime < 3)
        {
            return TimeOfDay.Midnight;
        }
        return 0;
    }

    readonly Dictionary<TimeOfDay, float> timeOfDayStart = new Dictionary<TimeOfDay, float>
    {
        {TimeOfDay.EarlyMorning,3 },
        {TimeOfDay.Dawn,6},
        {TimeOfDay.Morning,9},
        {TimeOfDay.Noon,12},
        {TimeOfDay.Afternoon,13},
        {TimeOfDay.Evening,15},
        {TimeOfDay.Dusk,18},
        {TimeOfDay.LateNight,21},
        {TimeOfDay.Midnight,0}
    };

   // public InputMethod inputMethod { get; private set; }

    public object DataToSave { get => gameData;}
    string id="05f";
    public string ID => id;


    
    InputUser user;

    //string mainScene;

    [SerializeField]
    UIDocument mainUI;
    [SerializeField]
    public AudioMixer audioMixer;
    UIDocument MainUI { get { if (mainUI == null) { mainUI= GameObject.Find("MainHud")?.GetComponent<UIDocument>(); return mainUI; } else { return mainUI; } } }

    public event Action<FishMonsterType> CaughtFish;
    public event Action WonFight;
    private void Awake()
    {

        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        Inn.InnVisited += (inn) => gameData.currentInnId=inn.innId;

        
        //InputUser.PerformPairingWithDevice()
        gameData.hasSeenFish = new bool[database.fishMonsters.Count];
        InputManager.Input.UI.Pause.Enable();
        InputManager.Input.UI.Pause.performed +=(x)=> 
        {
            PauseMenu.Pause();
        };
        if (mainUI != null)
        {
            MainUI.gameObject.SetActive(true);
        }


        //mainScene = SceneManager.GetActiveScene().name;
        DayTime = startingTime;

        foreach (var fish in testfisth)
        {
            CapturedFish(fish);
        }
        //PlayerFishventory.Fishies[0].ChangeName("SteveO");

        sun = FindObjectOfType<Light>().gameObject;
        for (int i = 0; i < startingItems.Length; i++)
        {
            PlayerInventory.AddItem(startingItems[i]);
        }
        InputManager.Input.Player.QuickSave.performed += (x) => SavingSystem.SaveGame(SavingSystem.SaveMode.QuickSave);
        InputManager.Input.Player.QuickLoad.performed += (x) => SavingSystem.LoadGame();

    }

    
    public void ResetLastInn()
    {
        gameData.currentInnId =null;
    }
    public void PlayerLost()
    {
        if(gameData.currentInnId == null|| gameData.currentInnId=="")
        {
            gameData.currentInnId = Inn.StarterInn.innId;
            Debug.Log(Inn.StarterInn);
        }
       
        lastInnVisited.Respawn();
        OnPlayerLost?.Invoke();
        Debug.Log("player has died");
        AdvanceTime(3);
        RestoreFish();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        //FishingMiniGame.SuccesfulFishing += (fish) => LoadCombatScene(new List<FishMonster>() { fish }, true);
    }

    // Update is called once per frame
    void Update()
    {

        DayNightCycle();
    }

    void DayNightCycle()
    {
        if (inCombat)
        {
            return;
        }
        if (sun ==null)
        {
            sun = FindObjectOfType<Light>()?.gameObject;
            if (sun == null)
            {
                return;
            }
        }
        if (tempTimeOfDayText != null)
        {
            tempTimeOfDayText.text = Regex.Replace(CurrentTimeOfDay.ToString(), "([A-Z0-9]+)", " $1").Trim();
        }


        sun.transform.eulerAngles = new Vector3(((DayTime * (360 / 24)) - 90), -90, 0);
        DayTime += Time.deltaTime / secondsPerHour;
        if (DayTime >= 24)
        {
            Day++;
            DayTime %= 24;
        }
        
    }
    /// <summary>
    /// advances time by in game hours not real time
    /// </summary>
    /// <param name="time"></param>
    /// 

    [DevConsoleCommand("AdvanceTime")]
    public static void AdvanceTimeCommand(string time)
    {
        Instance.AdvanceTime(float.Parse(time));
    }


    public void AdvanceTime(float time)
    {
        DayTime += time;
        StartCoroutine(FadeToBlack(() =>
        {
            if (DayTime >= 24)
            {
                Day += Mathf.FloorToInt((DayTime / 24F));
                DayTime %= 24;
            }
        }));
       
       
    }
    public void AdvanceTime(TimeOfDay timeOfDay)
    {

        float targetTime = timeOfDayStart[timeOfDay];
        if (this.CurrentTimeOfDay == timeOfDay)
        {
            return;
        }
        StartCoroutine(FadeToBlack(() =>
        {
            if (DayTime > targetTime)
            {
                Day++;
            }
            DayTime = targetTime;
        }));
       
        
    }
    IEnumerator FadeToBlack(Action completed)
    {
        ColorAdjustments colorAdjustments;
        
        if (postProcessing.TryGet(out colorAdjustments))
        {
            InputManager.DisablePlayer();
            print("start to fade");
            while (colorAdjustments.postExposure.value>-10)
            {
                print("should be fading");
                colorAdjustments.postExposure.value=Mathf.MoveTowards(colorAdjustments.postExposure.value, -20, 10 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1.5f);
            completed?.Invoke();
            
            while (colorAdjustments.postExposure.value < 0)
            {
                colorAdjustments.postExposure.value = Mathf.MoveTowards(colorAdjustments.postExposure.value, 0, 5 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            InputManager.EnablePlayer();
        }
       
    }
    public void RestoreFish()
    {
        PlayerFishventory.RestoreHealthAllFish();
    }

    [DevConsoleCommand("AddFish")]
    static public void CaptureFishByID(string id)
    {
        var fish = Instance.database.fishMonsters[int.Parse(id)];
        Instance.CapturedFish(fish);
    }
    [DevConsoleCommand("RemoveFish")]
    static public void RemoveFishFromInventory(string index)
    {
        Instance.PlayerFishventory.RemoveFish(Instance.PlayerFishventory.Fishies[int.Parse(index)]);
    }
    public void CapturedFish(FishMonsterType fishMonsterType)
    {
        CapturedFish(fishMonsterType.GenerateMonster(5),true);
        
    }
    public void CapturedFish(FishMonster fishMonster,bool ignoreQuest=false)
    {

        switch (PlayerFishventory.AddFish(fishMonster))
        {
            case -1:
                Debug.LogWarning("repeated fish");
                break;
            case 0:
                StoredFishventory.AddFish(fishMonster);
                break;
        }
        gameData.hasSeenFish[fishMonster.ID]= true;
        Debug.Log("is fish found:"+ gameData.hasSeenFish[fishMonster.ID]);
        if (!ignoreQuest)
        {
            CaughtFish?.Invoke(fishMonster.Type);
        }
       

    }
    [DevConsoleCommand("AddItem")]
    public static void AddItems(string name,string amount)
    {
        var item = Item.getItemById.Select((x) => x.Value).First((x)=>x.name==name);
        Instance.PlayerInventory.AddItem(item,int.Parse(amount));

    }


    [DevConsoleCommand("StartCombat")]
    public static void StartCombatByFishIds(string fish1, string level1, string fish2, string level2, string fish3, string level3)
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();
        if (int.Parse(fish1) >= 0)
        {
            fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fish1)].GenerateMonster(int.Parse(level1)));
        }
        if (int.Parse(fish2) >= 0)
        {
            fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fish2)].GenerateMonster(int.Parse(level2)));
        }
        if (int.Parse(fish3) >= 0)
        {
            fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fish3)].GenerateMonster(int.Parse(level3)));
        }
       

        Instance.LoadCombatScene(fishMonsters);
    }
    [DevConsoleCommand("StartCombat")]
    public static void StartCombatByFishIds(string fish1, string level1, string fish2, string level2)
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();
        if (int.Parse(fish1) >= 0)
        {
            fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fish1)].GenerateMonster(int.Parse(level1)));
        }
        if (int.Parse(fish2) >= 0)
        {
            fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fish2)].GenerateMonster(int.Parse(level2)));
        }


        Instance.LoadCombatScene(fishMonsters);
    }
    [DevConsoleCommand("StartCombat")]
    public static void StartCombatByFishIds(string fish1,string level1)
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();
        fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fish1)].GenerateMonster(int.Parse(level1)));



        Instance.LoadCombatScene(fishMonsters);
    }
    [DevConsoleCommand("StartCombatRandom")]
    public static void StartCombatRandom(string amount, string level)
    {
        int _amount=int.Parse(amount);
        int _level=int.Parse(level);

        List<FishMonster> fishMonsters = new List<FishMonster>();
        for(int i = 0; i < _amount; i++)
        {
            fishMonsters.Add(Instance.database.fishMonsters[UnityEngine.Random.Range(0,Instance.database.FishMonsters.Count)].GenerateMonster());
        }
        Instance.LoadCombatScene(fishMonsters);
    }
    //public static void StartCombatByFishIds(params string[] fishIds)
    //{
    //    List<FishMonster> fishMonsters = new List<FishMonster>();
    //    foreach (string fishId in fishIds)
    //    {
    //        fishMonsters.Add(Instance.database.fishMonsters[int.Parse(fishId)].GenerateMonster());
    //    }
    //    Instance.LoadCombatScene(fishMonsters);
    //}
    public void LoadCombatScene(List<FishMonster> enemyFishes, bool rewardFish = false)
    {
        gameData.currentScene = SceneManager.GetActiveScene().buildIndex;
        SavingSystem.SaveGame(SavingSystem.SaveMode.AutoSave);
        //mainEventSystem.enabled = false;
        CombatManager.NewCombat(enemyFishes, rewardFish);
        inCombat = true;
        SceneManager.sceneLoaded += SceneLoaded;
        
       
    }

    public void LoadBossCombatScene(List<FishMonster> enemyFishes, string objectid)
    {
        gameData.currentScene = SceneManager.GetActiveScene().buildIndex;
        SavingSystem.SaveGame(SavingSystem.SaveMode.AutoSave);
        //mainEventSystem.enabled = false;
        CombatManager.NewCombat(enemyFishes, false);
        inCombat = true;
        SceneManager.sceneLoaded += SceneLoaded;
        OnPlayerLost += OnPlayerLostFight;
        WonFight += PlayerWonFight;

        void OnPlayerLostFight()
        {
            FindObjectsOfType<BossFightStart>(true).First((x) => x.ID == objectid).gameObject.SetActive(true);
        }
        void PlayerWonFight()
        {
            OnPlayerLost -= OnPlayerLostFight;
            WonFight -= PlayerWonFight;
        }

    }
    public void CombatEnded(Team winningTeam)
    {

        if (winningTeam == Team.enemy)
        {
            SceneManager.sceneLoaded += SceneLoadedLost;
        }
        else
        {
            WonFight?.Invoke();
        }
        SavingSystem.SaveSelf(this,this.gameObject.scene.buildIndex);
        var questTracker = FindObjectOfType<QuestTracker>();
        SavingSystem.SaveSelf(questTracker, questTracker.gameObject.scene.buildIndex);
        //InputManager.Input.UI.Disable();
        //InputManager.DisableCombat();
        SceneManager.LoadScene(gameData.currentScene);

        
        inCombat = false;
       
    }

    void SceneLoadedLost(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name=="Main Scene")
        {
            FindAnyObjectByType<SceneLoader>().AllScenesLoaded += () =>
            {
                PlayerLost();
                SceneManager.sceneLoaded -= SceneLoadedLost;
            };
        }
        
       
    }

    void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "BattleScene 1")
        {
            inCombat = true;
            
        }
        else if(arg0.buildIndex == gameData.currentScene)
        {
            print("should load");
            SavingSystem.LoadGame();
            SceneManager.sceneLoaded -= SceneLoaded;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            //InputManager.EnablePlayer();

        }
        
    }

    public void Load(string json)
    {
        gameData =new();
        gameData = JsonUtility.FromJson<GameData>(json);
    }

    private void OnDestroy()
    {
        
    }
    
}
