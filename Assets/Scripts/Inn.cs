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

    public InnDialogue dialogue;

    public bool Interact()
    {
        //dialogue.DisplayFirstOption();
        InnVisited?.Invoke(this);
        if (GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Night))
        {
            dialogue.DisplayFirstOption();
            //dialogue.ShowUI();
            print("should sleep");
            GameManager.Instance.AdvanceTime(GameManager.TimeOfDay.Dawn);
            GameManager.Instance.RestoreFish();
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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
