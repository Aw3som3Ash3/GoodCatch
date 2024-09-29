using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;

public class FishingMiniGame : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform circle;
    Floater floater;

    public static Action<FishMonster> SuccesfulFishing;

    float score;

    int difficulty;
    FishMonsterType fishMonster;



    public void Initiate(Floater floater)
    {
        this.floater = floater;
        virtualCamera.Priority = 15;
        fishMonster = FishDatabase.instance.GetRandom();
        print(fishMonster);

    }

    void ExitFishing()
    {

    }

    private void Update()
    {

        //floater.transform.position = new Vector3(circle.position.x, floater.transform.position.y, circle.position.z);
    }

    void FishingSucces()
    {
        SuccesfulFishing?.Invoke(fishMonster.GenerateMonster());
    }
    //Start Mini Game
    //Detect Win/Loss
    //Output Fish
    //Start Battle
}
