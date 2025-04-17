using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingMiniGame : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    //[SerializeField] Transform circle;
    Floater floater;

    public static event Action SuccesfulFishing;

    float score;

    int difficulty;
    //[SerializeField]
   // FishMonsterType fishMonster;
    [SerializeField]
    GameObject fishToCatchPrefab;
    FishToCatch fishToCatch;

    [SerializeField] GameObject minigame;
    private FishingGameUI minigameUI;
    [SerializeField]
    LayerMask fishZones;

    public Action OnCancel;

    FishingPromptUI prompt;
    FishZone fishZone;

    void Awake()
    {
        prompt = FindObjectOfType<FishingPromptUI>(true);
    }

    public void Initiate(Floater floater)
    {
        prompt.ShowUI();

        this.floater = floater;
        virtualCamera.Priority = 15;
        InputManager.Input.Fishing.Enable();
        InputManager.Input.Fishing.Hook.performed += OnHook;
        InputManager.Input.Fishing.Exit.performed += OnExit;
        
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position,Vector3.down,out hit,100, fishZones))
        {
            Debug.Log(" hit fishZones");
            fishZone = hit.collider.GetComponent<FishZone>();
            fishToCatch = fishZone.GetFishObect();
            fishToCatch.StartCatching(floater.transform);
            //Invoke("SpawnFish", UnityEngine.Random.Range(0,1f));
        }
        
    }

    void OnExit(InputAction.CallbackContext context)
    {
        OnCancel?.Invoke();
        ExitFishing();
    }
    void ExitFishing()
    {
        prompt.DisableUI();
        if (fishToCatch != null)
        {
            fishToCatch.SetIdle();
        }
        
        InputManager.Input.Fishing.Disable();
        Destroy(this.gameObject);
        InputManager.Input.Fishing.Hook.performed -= OnHook;
        InputManager.Input.Fishing.Exit.performed -= OnExit;
        Destroy(floater.gameObject);
        //InputManager.EnablePlayer();
    }

    private void Update()
    {
        //floater.transform.position = new Vector3(circle.position.x, floater.transform.position.y, circle.position.z);
    }

    //void SpawnFish()
    //{
    //    Debug.Log("shoulde spawn");
    //    fishToCatch = Instantiate(fishToCatchPrefab, this.transform).GetComponent<FishToCatch>();
    //    fishToCatch.transform.Translate(Vector3.down*2);
    //    fishToCatch.StartCatching(floater.transform);
    //}

    void OnHook(InputAction.CallbackContext context)
    {
        if (fishToCatch!=null&&fishToCatch.CatchFish())
        {
            //StartMinigame();
            FishingSuccess();
        }
        else
        {
            //should do different fail animation or reeling nothing
            OnCancel?.Invoke();
            ExitFishing();
        }
    }

    void FishingSuccess()
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();
        int num = UnityEngine.Random.Range(1, Mathf.Clamp(GameManager.Instance.PlayerFishventory.Fishies.Count+1,0,4));
        for (int i = 0; i < num; i++)
        {
            fishMonsters.Add(fishZone.GetRandomFish());
        }
        InputManager.Input.Fishing.Disable();
        SuccesfulFishing?.Invoke();
        GameManager.Instance.LoadCombatScene(fishMonsters, true);
        //ExitFishing();

    }


    void StartMinigame()
    {
        if (minigameUI == null)
        {
            minigameUI = Instantiate(minigame, FindObjectOfType<Canvas>().transform).GetComponent<FishingGameUI>();
        }

        minigameUI.StartMinigame(1, (b) => { if(b) { FishingSuccess(); } });
    }

    //Start Mini Game
    //Detect Win/Loss
    //Output Fish
    //Start Battle
}
