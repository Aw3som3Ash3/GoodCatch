using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static CombatManager;

public class GameManager : MonoBehaviour,ISaveable
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

        public GameData(int partySize)
        {
            PlayerFishventory = new Fishventory(partySize);
            PlayerInventory = new ItemInventory();
            StoredFishventory = new Fishventory();
            dayTime =0;
        }
    }
    GameData gameData=new GameData(7);
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
    Inn lastInnVisited;

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

    public InputMethod inputMethod { get; private set; }

    public object DataToSave { get => gameData;}
    string id="05f";
    public string ID => id;


    public Action<InputMethod> OnInputChange;
    InputUser user;

    String mainScene;

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
        Inn.InnVisited += (inn) => lastInnVisited = inn;

        user = InputUser.CreateUserWithoutPairedDevices();
        InputUser.listenForUnpairedDeviceActivity = 1;
        InputUser.onUnpairedDeviceUsed += OnDeviceChange;
        InputUser.onChange += OnDeviceChange;
        //InputUser.PerformPairingWithDevice()
       


    }

    private void OnDeviceChange(InputControl control, InputEventPtr ptr)
    {
        Debug.Log(control.device);

        //user.UnpairDevices();
        InputUser.PerformPairingWithDevice(control.device, user, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
        //throw new NotImplementedException();
    }

    private void OnDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {

        Debug.Log(user);
        Debug.Log(device);
        if (change == InputUserChange.DevicePaired)
        {
            
            if (device is Gamepad)
            {
                Debug.Log("is gamepad");
                inputMethod = InputMethod.controller;
            }
            else if (device is Mouse || device is Keyboard)
            {
                inputMethod = InputMethod.mouseAndKeyboard;
                Debug.Log("is m&k");
            }
            OnInputChange?.Invoke(inputMethod);
        }else if(change == InputUserChange.Removed)
        {
            //user.UnpairDevice(device);
        }
        
    }


    public void PlayerLost()
    {
        FindObjectOfType<PlayerController>().transform.position=lastInnVisited.GetRepsawnPoint();
        AdvanceTime(3);
        RestoreFish();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainScene = SceneManager.GetActiveScene().name;
        DayTime = startingTime;
        foreach(var fish in testfisth)
        {
            CapturedFish(fish);
        }
        PlayerFishventory.Fishies[0].ChangeName("SteveO starter fish");

        sun = FindObjectOfType<Light>().gameObject;
        for(int i=0;i<startingItems.Length;i++)
        {
            PlayerInventory.AddItem(startingItems[i]);
        }
        InputManager.Input.Player.QuickSave.performed +=(x)=>SavingSystem.SaveGame(SavingSystem.SaveMode.QuickSave);
        InputManager.Input.Player.QuickLoad.performed +=(x)=>SavingSystem.LoadGame();
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
    public void CapturedFish(FishMonsterType fishMonsterType)
    {
        PlayerFishventory.AddFish(fishMonsterType.GenerateMonster());

    }
    public void CapturedFish(FishMonster fishMonster)
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

    }
    public void LoadCombatScene(List<FishMonster> enemyFishes, bool rewardFish = false)
    {
        SavingSystem.SaveGame(SavingSystem.SaveMode.AutoSave);
        //mainEventSystem.enabled = false;
        CombatManager.NewCombat(enemyFishes, rewardFish);
        inCombat = true;
        SceneManager.sceneLoaded += SceneLoaded;
        
       
    }
    public void CombatEnded(Team winningTeam)
    {

        if (winningTeam == Team.enemy)
        {
            PlayerLost();
        }
        SavingSystem.SaveSelf(this);
        //InputManager.Input.UI.Disable();
        //InputManager.DisableCombat();
        SceneManager.LoadScene(mainScene);
        
        inCombat = false;
       
    }
    void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "BattleScene 1")
        {
            inCombat = true;
            
        }
        else if(arg0.name == mainScene)
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
        gameData = JsonUtility.FromJson<GameData>(json);
    }
}
