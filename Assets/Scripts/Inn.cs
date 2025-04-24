using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Inn : MonoBehaviour, IInteractable
{
    public string StationName => "Inn";

    public bool IsInteractable => true;

    [SerializeField]
    public string innId;
    [SerializeField]
    Transform respawnPoint;
    public static Action<Inn> InnVisited;
    public static Inn StarterInn;
    public InnDialogue dialogue;
    [SerializeField]
    DockingZone dockingZone;
    public static Dictionary<string, Inn> innIds = new();

    [SerializeField]
    InputAction debugTeleport;

    private void TeleportDebug(InputAction.CallbackContext context)
    {
        Respawn();
    }



    [SerializeField]
    bool isStartInn;

    void Awake()
    {
       
        dialogue =FindObjectOfType<InnDialogue>(true);
        if (isStartInn)
        {
            StarterInn = this;
            Debug.Log($"{this} is the new starter inn {StarterInn}");
            //GameManager.Instance.ga = this;
            
        }
        if (innIds == null)
        {
            innIds = new Dictionary<string, Inn>();
        }
        innIds[innId] = this;
        debugTeleport.performed += TeleportDebug;
        debugTeleport.Enable();
    }
    public static void RemoveInnFromDictionary(string innId)
    {
        innIds.Remove(innId);
    }
    public void Respawn()
    {
        if (dockingZone != null)
        {
            dockingZone.ResetShip();
        }
        Debug.Log(PlayerController.player);
        Debug.Log(innId);
        Debug.Log(respawnPoint.position);
        PlayerController.player.SetPosition(respawnPoint.position);
        PlayerController.player.SetVisibility(true);
    }
    public bool Interact()
    {
        /*InnVisited?.Invoke(this);
        if (GameManager.Instance.CurrentTimeOfDay.HasFlag(GameManager.TimeOfDay.Night))
        {
            dialogue.DisplayFirstOption(SleepThroughNight);
            return true;
        }
       print("it is not night time could not sleep");
       dialogue.CantSleepMessage();
       return false;*/

       dialogue.DisplayFirstOption(SleepThroughNight);
       return true;
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
