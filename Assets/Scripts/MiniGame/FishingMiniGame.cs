using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;

public class FishingMiniGame : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    //[SerializeField] Transform circle;
    Floater floater;

    public static Action SuccesfulFishing;

    float score;

    int difficulty;
    [SerializeField]
    FishMonsterType fishMonster;
    [SerializeField]
    GameObject fishToCatchPrefab;
    FishToCatch fishToCatch;
    

    public void Initiate(Floater floater)
    {
        this.floater = floater;
        virtualCamera.Priority = 15;
        //fishMonster = FishDatabase.instance.GetRandom();
        InputManager.Input.Fishing.Enable();
        InputManager.Input.Fishing.Hook.performed += OnHook;
        InputManager.Input.Fishing.Exit.performed += OnExit;
        print(fishMonster);
        Invoke("SpawnFish", UnityEngine.Random.Range(0,2f));
    }

    void OnExit(InputAction.CallbackContext context)
    {
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
        if (fishToCatch.CatchFish()) 
        {
            FishingSuccess();
            
        }
    }

    void FishingSuccess()
    {
        List<FishMonster> fishMonsters= new List<FishMonster>();
        int num = UnityEngine.Random.Range(1, 1);
        for(int i = 0; i < num; i++) 
        {
            fishMonsters.Add(fishMonster.GenerateMonster());
        }
        GameManager.Instance.LoadCombatScene(fishMonsters,true);
        SuccesfulFishing?.Invoke();
        ExitFishing();
    }
    //Start Mini Game
    //Detect Win/Loss
    //Output Fish
    //Start Battle
}
