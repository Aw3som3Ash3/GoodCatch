using UnityEngine;
using UnityEngine.InputSystem;

public class ShipControls : Station
{
    [SerializeField]
    ShipSimulator ship;
    float turnValue;
    float sailValue;
    [SerializeField]
    Transform wheel;
    [SerializeField]
    float turnSpeed;

    public override void MoveInput(Vector2 input)
    {
        turnValue = input.x;
        sailValue = input.y;
        ship.AdjustTurn(turnValue);

        ship.AdjustSails(sailValue);
    }

    public override void Use()
    {
    }


    // Start is called before the first frame update
    void Start()
    {

    }
    public override bool Interact()
    {
        if (!base.Interact())
        {
            return false;
        }
        InputManager.Input.Ship.Enable();
        InputManager.Input.Ship.Move.performed += OnMove;
        InputManager.Input.Ship.Exit.performed += (InputAction.CallbackContext context) => LeaveSation();
        return true;

    }
    void OnMove(InputAction.CallbackContext context)
    {
        MoveInput(context.ReadValue<Vector2>());
    }
    public override void LeaveSation()
    {
        InputManager.Input.Ship.Disable();
        base.LeaveSation();

    }
    // Update is called once per frame
    void Update()
    {

        wheel.eulerAngles = new Vector3(wheel.eulerAngles.x, wheel.eulerAngles.y, -ship.turnRatio * 360);

    }
}
