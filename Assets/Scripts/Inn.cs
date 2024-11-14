using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inn : MonoBehaviour, IInteractable
{
    public string StationName => "Inn";
    [SerializeField]
    Transform respawnPoint;
    public static Action<Inn> InnVisited;
    public bool Interact()
    {
        InnVisited?.Invoke(this);
        if (GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Night))
        {
            print("should sleep");
            GameManager.Instance.AdvanceTime(GameManager.TimeOfDay.Dawn);
            GameManager.Instance.RestoreFish();
            return true;
        }
       print("it is not night time could not sleep");
       return false;
    }
    public Vector3 GetRepsawnPoint()
    {
        return respawnPoint.position;
    }
    public void SleepThroughNight()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
