using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Inn : MonoBehaviour, IInteractable
{
    public string StationName => "Inn";
    [SerializeField]
    Transform respawnPoint;
    public static Action<Inn> InnVisited;
    public static Inn StarterInn;
    public InnDialogue dialogue;

    bool isStartInn;

    void Awake()
    {
        dialogue=FindObjectOfType<InnDialogue>();
        if (isStartInn)
        {
            starterInn = this;
        }
    }

    public bool Interact()
    {
        InnVisited?.Invoke(this);
        if (GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Night))
        {
            dialogue.DisplayFirstOption(SleepThroughNight);
            return true;
        }
       print("it is not night time could not sleep");
       dialogue.CantSleepMessage();
       return false;
    }
    public Vector3 GetRepsawnPoint()
    {
        return respawnPoint.position;
    }

    public void SleepThroughNight()
    {
        print("should sleep");
        GameManager.Instance.AdvanceTime(GameManager.TimeOfDay.Dawn);
        GameManager.Instance.RestoreFish();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
