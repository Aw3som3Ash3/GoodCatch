using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Station : MonoBehaviour, IInteractable
{
    static public Action<Station,Transform> StationInteracted;
    bool beingUsed = false;
    [SerializeField]
    Transform stationZone;
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    string stationName;
    public Vector3 StationPosition { get { return stationZone.position; } }
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera.Priority = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void MoveInput(Vector2 input);
    public void LeaveSation() 
    {
        virtualCamera.Priority = 0;


    }


    public bool Interact()
    {
        if (beingUsed)
        {
            return false;
        }
       StationInteracted(this,stationZone);
        virtualCamera.Priority = 11;
        return true;


    }

    public string StationName()
    {
        return "E-" + stationName;
    }
    public abstract void Use();
}