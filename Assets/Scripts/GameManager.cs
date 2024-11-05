using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Fishventory PlayerFishventory { get; private set; } = new Fishventory(7);
    public ItemInventory PlayerInventory { get; private set; } = new ItemInventory();

    List<FishMonster> fishesToFight;
    float dayTime;
    public int Day { get; private set; }
    [SerializeField]
    [Range(0,24)]
    float startingTime;
    [SerializeField]
    [Min(1)]
    float secondsPerHour = 1;
    GameObject sun;
    [SerializeField]
    FishMonsterType testfisth;
    bool rewardFish;
    EventSystem mainEventSystem;
    [SerializeField]
    TextMeshProUGUI tempTimeOfDayText;

    [SerializeField]
    VolumeProfile postProcessing;

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
    public TimeOfDay CurrentTimeOfDay => GetTimeOfDay(dayTime);
    public TimeOfDay GetTimeOfDay(float time)
    {
        if (dayTime >= 3 && dayTime < 6)
        {
            return TimeOfDay.EarlyMorning;
        }
        else if (dayTime >= 6 && dayTime < 9)
        {
            return TimeOfDay.Dawn;
        }
        else if (dayTime >= 9 && dayTime < 12)
        {
            return TimeOfDay.Morning;
        }
        else if (dayTime >= 9 && dayTime < 12)
        {
            return TimeOfDay.Morning;
        }
        else if (dayTime >= 12 && dayTime < 13)
        {
            return TimeOfDay.Noon;
        }
        else if (dayTime >= 13 && dayTime < 15)
        {
            return TimeOfDay.Afternoon;
        }
        else if (dayTime >= 15 && dayTime < 18)
        {
            return TimeOfDay.Evening;
        }
        else if (dayTime >= 18 && dayTime < 21)
        {
            return TimeOfDay.Dusk;
        }
        else if (dayTime >= 21 && dayTime < 24)
        {
            return TimeOfDay.LateNight;
        }
        else if (dayTime >= 0 && dayTime < 3)
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

        FishingMiniGame.SuccesfulFishing += () => { mainEventSystem.enabled = true; };
    }
    // Start is called before the first frame update
    void Start()
    {
        dayTime = startingTime;
        CapturedFish(testfisth);
        CapturedFish(testfisth);
        PlayerFishventory.Fishies[0].ChangeName("SteveO starter fish");

        mainEventSystem = EventSystem.current;
        sun = FindObjectOfType<Light>().gameObject;
        //FishingMiniGame.SuccesfulFishing += (fish) => LoadCombatScene(new List<FishMonster>() { fish }, true);
    }

    // Update is called once per frame
    void Update()
    {

        DayNightCycle();
    }

    void DayNightCycle()
    {
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


        sun.transform.eulerAngles = new Vector3(((dayTime * (360 / 24)) - 90), -90, 0);
        dayTime += Time.deltaTime / secondsPerHour;
        if (dayTime >= 24)
        {
            Day++;
            dayTime %= 24;
        }
    }
    /// <summary>
    /// advances time by in game hours not real time
    /// </summary>
    /// <param name="time"></param>
    public void AdvanceTime(float time)
    {
        dayTime += time;
        StartCoroutine(FadeToBlack(() =>
        {
            if (dayTime >= 24)
            {
                Day += Mathf.FloorToInt((dayTime / 24F));
                dayTime %= 24;
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
            if (dayTime > targetTime)
            {
                Day++;
            }
            dayTime = targetTime;
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
        PlayerFishventory.AddFish(fishMonster);

    }
    public void LoadCombatScene(List<FishMonster> enemyFishes, bool rewardFish = false)
    {
        mainEventSystem.enabled = false;
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        
        fishesToFight = enemyFishes;
        SceneManager.sceneLoaded += SetUpCombat;
        this.rewardFish = rewardFish;
    }

    private void SetUpCombat(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "BattleScene")
        {

            GameObject.FindObjectOfType<CombatManager>().NewCombat(PlayerFishventory.Fishies.ToList(), fishesToFight, rewardFish);
        }
        SceneManager.sceneLoaded -= SetUpCombat;
        //throw new NotImplementedException();
    }


}
