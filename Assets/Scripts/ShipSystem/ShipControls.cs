using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipControls : Station
{
    [SerializeField]
    ShipSimulator ship;
    float turnValue;
    float sailValue;
    DockingZone currentDock;
    [SerializeField]
    LayerMask dockLayer;

    [SerializeField]
    FishingRod fishingRod;
    [SerializeField]
    GameObject rodBase;

    public event Action<bool> OnInteract;

    public override void MoveInput(Vector2 input)
    {
        turnValue = input.x;
        sailValue = input.y;
        ship.AdjustTurn(sailValue>=0? turnValue:-turnValue);
        ship.AdjustSails(sailValue);
        
    }

    public override void Use()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
        InputManager.Input.Ship.Disable();
    }
    public override bool Interact()
    {
        if (!base.Interact())
        {
            return false;
        }
        if (currentDock != null)
        {
            currentDock.UndockShip();
            currentDock = null;
        }
        InputManager.Input.Ship.Enable();
        InputManager.Input.Ship.Move.performed += OnMove;
        InputManager.Input.Ship.Exit.performed += OnLeaveStation;
        InputManager.Input.Ship.Fish.performed += OnCast;
        InputManager.DisablePlayer();
        //InputManager.Input.Ship.Exit.performed += OnLeaveStation;
        PlayerController.player.SetVisibility(false);
        OnInteract?.Invoke(true);
        return true;

    }

    private void OnCast(InputAction.CallbackContext context)
    {
        turnValue = 0;

        sailValue = 0;
        ship.AdjustTurn(turnValue);

        ship.AdjustSails(sailValue);
        InputManager.Input.Ship.Disable();
        fishingRod.CastLine(Vector3.zero, 0, () => { InputManager.Input.Ship.Enable(); });
        //throw new NotImplementedException();
    }

    void OnLeaveStation(InputAction.CallbackContext context)
    {
        foreach (Collider col in Physics.OverlapSphere(ship.transform.position, 10, dockLayer))
        {
            Debug.Log(col);
            if (col.GetComponent<DockingZone>() != null)
            {
                col.GetComponent<DockingZone>()?.DockShip(ship, true, () => { LeaveSation(); currentDock = col.GetComponent<DockingZone>(); PlayerController.player.SetVisibility(true); });
                
                turnValue = 0;

                sailValue = 0;
                ship.AdjustTurn(turnValue);
                InputManager.EnablePlayer();
                ship.AdjustSails(sailValue);
                ship.PhysicSim.velocity = Vector3.zero;
                ship.PhysicSim.angularVelocity = Vector3.zero;
                
                return;
            }

        }
       
       
       
       
    }
    void OnMove(InputAction.CallbackContext context)
    {
        MoveInput(context.ReadValue<Vector2>());
    }
    public override void LeaveSation()
    {
       
       
        InputManager.Input.Ship.Disable();
        InputManager.Input.Ship.Move.performed -= OnMove;
        InputManager.Input.Ship.Exit.performed -= OnLeaveStation;
        InputManager.Input.Ship.Fish.performed -= OnCast;
        OnInteract?.Invoke(false);
        base.LeaveSation();

    }

    private void OnDestroy()
    {
        InputManager.Input.Ship.Disable();
        InputManager.Input.Ship.Move.performed -= OnMove;
        InputManager.Input.Ship.Exit.performed -= OnLeaveStation;
        InputManager.Input.Ship.Fish.performed -= OnCast;
    }
    // Update is called once per frame
    void Update()
    {

        //wheel.eulerAngles = new Vector3(wheel.eulerAngles.x, wheel.eulerAngles.y, -ship.turnRatio * 360);

    }
}
