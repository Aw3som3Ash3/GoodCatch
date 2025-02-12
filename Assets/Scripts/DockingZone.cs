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


    public void DockShip(ShipSimulator ship)
    {
        this.ship = ship;
        ship.transform.position=dockingPosition.position;
        ship.transform.rotation=dockingPosition.rotation;
        ship.AnchorShip();
        //joint=ship.PhysicSim.gameObject.AddComponent<FixedJoint>();

    }

    public void UndockShip()
    {
        ship.UnAnchorShip();
        ship = null;
        Destroy(joint);
        joint = null;
       

    }
}
