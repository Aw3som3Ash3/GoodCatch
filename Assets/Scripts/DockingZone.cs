using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DockingZone : SaveableObject, IInteractable
{

    [SerializeField]
    Transform dockingPosition;
    
    ShipSimulator ship;
    FixedJoint joint;
    [SerializeField]
    bool firstDock;
    [SerializeField]
    Transform dockExit;
    [SerializeField]
    Mesh shipMesh;
    [SerializeField]
    Vector3 gizmoOffset;

    public string StationName => ship!=null? "Enter Ship":"";


    public override object DataToSave =>ship!=null? ship.ID:null;

    public bool IsInteractable => ship!=null;



    // Start is called before the first frame update
    void Start()
    {
        ship = null;
        if (firstDock)
        {
            ResetShip();
        }
        //DockShip(ship);
    }

    public void ResetShip()
    {
        DockShip(FindObjectOfType<ShipSimulator>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DockShip(ShipSimulator ship)
    {

        this.ship = ship;
        ship.shipControls.LeaveSation();
        ship.transform.position=dockingPosition.position;
        ship.transform.rotation=dockingPosition.rotation;
        //joint=ship.PhysicSim.gameObject.AddComponent<FixedJoint>();

    }

    public void DockShip(ShipSimulator ship,bool resetPosition=false, Action callback = null)
    {
        StopCoroutine(DockShipAnimation(callback, resetPosition));
        this.ship = ship;
        //ship.AnchorShip();
        StartCoroutine(DockShipAnimation(callback, resetPosition));
      
        //joint=ship.PhysicSim.gameObject.AddComponent<FixedJoint>();

    }

    IEnumerator DockShipAnimation(Action completed,bool resetPosition = false)
    {

        ship.PhysicSim.isKinematic = true;
        while (Vector3.Distance(ship.transform.position, dockingPosition.position)>0.5f || ship.transform.rotation != dockingPosition.rotation)
        {
            yield return new WaitForFixedUpdate();
            ship.transform.position=Vector3.MoveTowards(ship.transform.position,dockingPosition.position,10*Time.fixedDeltaTime);
            ship.transform.rotation = Quaternion.RotateTowards(ship.transform.rotation, dockingPosition.rotation,90 * Time.fixedDeltaTime);
        }
        if (resetPosition)
        {
            PlayerController.player.SetPosition(dockExit.position);
        }
        ship.PhysicSim.isKinematic = false;
        completed?.Invoke();
    }


    public void UndockShip()
    {
        //ship.UnAnchorShip();
        StopCoroutine("DockShipAnimation");
        ship.PhysicSim.isKinematic = false;
        ship = null;
        Destroy(joint);
        joint = null;
       

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(shipMesh,0 ,dockingPosition.transform.position+gizmoOffset, dockingPosition.transform.rotation);
      
    }

    public bool Interact()
    {
        if (ship != null)
        {
            ship.GetComponentInChildren<ShipControls>().Interact();
            return true;
        }
        else
        {
            return false;
        }
      
    }

    public override void Load(string json)
    {
        if (json == null)
        {
            return;
        }
        var shipId=JsonUtility.FromJson<string>(json);
        if (shipId == null||shipId=="")
        {
            return;
        }
        //var ship = FindObjectsOfType<ShipSimulator>().FirstOrDefault((x)=>x.ID==shipId);
        //this.ship = ship;
        //DockShip(ship);
    }
}
