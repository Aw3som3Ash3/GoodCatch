using Cinemachine;
using System;
using UnityEngine;

public abstract class Station : MonoBehaviour, IInteractable
{
    static public Action<Station, Transform> StationInteracted;
    static public Action<Station> LeftStation;
    //static public Action<Station,Transform> StationInteracted;
    bool beingUsed = false;
    [SerializeField]
    protected Transform stationZone;
    [SerializeField]
    protected CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    string stationName;
    public Vector3 StationPosition { get { return stationZone.position; } }
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera.Priority = 0;
        virtualCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public abstract void MoveInput(Vector2 input);
    virtual public void LeaveSation()
    {
        LeftStation?.Invoke(this);
        virtualCamera.gameObject.SetActive(false);
        virtualCamera.Priority = 0;


    }


    virtual public bool Interact()
    {
        if (beingUsed)
        {
            return false;
        }
        StationInteracted?.Invoke(this, stationZone);
        virtualCamera.gameObject.SetActive(true);
        virtualCamera.Priority = 11;
        return true;


    }

    public string StationName => stationName;

    public bool IsInteractable => !beingUsed;

    public abstract void Use();
}
