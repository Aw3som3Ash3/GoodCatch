using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingZone : MonoBehaviour
{

    [SerializeField]
    Transform dockingPosition;
    
    ShipSimulator ship;
    FixedJoint joint;
    [SerializeField]
    bool firstDock;
    [SerializeField]
    Transform dockExit;
    // Start is called before the first frame update
    void Start()
    {
        if (firstDock)
        {
            DockShip(FindObjectOfType<ShipSimulator>());
        }
        //DockShip(ship);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DockShip(ShipSimulator ship,bool resetPosition=false)
    {
        this.ship = ship;
        ship.transform.position=dockingPosition.position;
        ship.transform.rotation=dockingPosition.rotation;
        //ship.AnchorShip();
        if (resetPosition)
        {
            PlayerController.player.SetPosition(dockExit.position);
        }
        //joint=ship.PhysicSim.gameObject.AddComponent<FixedJoint>();

    }

    public void UndockShip()
    {
        //ship.UnAnchorShip();
        ship = null;
        Destroy(joint);
        joint = null;
       

    }
}
