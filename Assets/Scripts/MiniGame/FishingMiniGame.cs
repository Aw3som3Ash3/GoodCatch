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
    FishMonsterType fishMonster;
    [SerializeField]
    GameObject fishToCatchPrefab;
    FishToCatch fishToCatch;

    [SerializeField] GameObject minigame;
    private FishingGameUI minigameUI;
    [SerializeField]
    LayerMask fishZones;

    public Action OnCancel;
    public void Initiate(Floater floater)
    {
        this.floater = floater;
        virtualCamera.Priority = 15;
        InputManager.Input.Fishing.Enable();
        InputManager.Input.Fishing.Hook.performed += OnHook;
        InputManager.Input.Fishing.Exit.performed += OnExit;
        
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position,Vector3.down,out hit,100, fishZones))
        {
            fishMonster=hit.collider.GetComponent<FishZone>().GetRandomFish(SuccesfulFishing);
            Invoke("SpawnFish", UnityEngine.Random.Range(0, 10f));
        }
        
    }

    void OnExit(InputAction.CallbackContext context)
    {
        OnCancel?.Invoke();
        ExitFishing();
    }
    void ExitFishing()
    {
        InputManager.Input.Fishing.Disable();
        Destroy(this.gameObject);
        InputManager.Input.Fishing.Hook.performed -= OnHook;
        InputManager.Input.Fishing.Exit.performed -= OnExit;
        Destroy(floater.gameObject);
        InputManager.EnablePlayer();
    }

    private void Update()
    {
        //floater.transform.position = new Vector3(circle.position.x, floater.transform.position.y, circle.position.z);
    }

    void SpawnFish()
    {
        fishToCatch = Instantiate(fishToCatchPrefab, this.transform).GetComponent<FishToCatch>();
        fishToCatch.SetFish(fishMonster, floater.transform);
    }

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
        int num = UnityEngine.Random.Range(1, 2);
        for (int i = 0; i < num; i++)
        {
            fishMonsters.Add(fishMonster.GenerateMonster());
        }
        SuccesfulFishing?.Invoke();
        GameManager.Instance.LoadCombatScene(fishMonsters, true);
        ExitFishing();

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
