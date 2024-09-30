using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Fishventory playerFishventory { get; private set; } = new Fishventory(7);

    List<FishMonster> fishesToFight;

    [SerializeField]
    FishMonsterType testfisth;
    bool rewardFish;
    EventSystem mainEventSystem;
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
        CapturedFish(testfisth);
        playerFishventory.Fishies[0].ChangeName("SteveO starter fish");
        mainEventSystem=EventSystem.current;
        //FishingMiniGame.SuccesfulFishing += (fish) => LoadCombatScene(new List<FishMonster>() { fish }, true);
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void CapturedFish(FishMonsterType fishMonsterType)
    {
        playerFishventory.AddFish(fishMonsterType.GenerateMonster());
        
    }
    public void CapturedFish(FishMonster fishMonster)
    {
        playerFishventory.AddFish(fishMonster);

    }
    public void LoadCombatScene(List<FishMonster> enemyFishes,bool rewardFish=false)
    {
        mainEventSystem.enabled = false;
        SceneManager.LoadScene("BattleScene",LoadSceneMode.Additive);
        fishesToFight = enemyFishes;
        SceneManager.sceneLoaded += SetUpCombat;
        this.rewardFish = rewardFish;
    }

    private void SetUpCombat(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "BattleScene")
        {
     
            GameObject.FindObjectOfType<CombatManager>().NewCombat(playerFishventory.Fishies.ToList(), fishesToFight,rewardFish);
        }
        SceneManager.sceneLoaded -= SetUpCombat;
        //throw new NotImplementedException();
    }

    
}
