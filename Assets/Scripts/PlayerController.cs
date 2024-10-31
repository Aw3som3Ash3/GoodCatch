using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    GoodCatchInputs.PlayerActions inputs;
    InputAction moveAction;
    InputAction lookAction;
    CharacterController characterController;
    [SerializeField]
    Transform cameraRig;
    [SerializeField]
    float moveSpeed, accel, jumpStrength;
    [SerializeField]
    float maxPitch, minPitch;
    Vector3 velocity;
    float fallSpeed;
    const float Gravity = -9.8f;
    const float rotSensitivity = 10;
    [SerializeField]
    float mouseSensitiviy;
    Vector2 rotVelocity;
    [SerializeField]
    LayerMask interactionLayer;


    [SerializeField]
    FishingRod fishingRod;
    [SerializeField] TextMeshProUGUI InteractionUI;


    bool inStation;
    private void Awake()
    {
        inputs = InputManager.Input.Player;
        InputManager.EnablePlayer();
        moveAction = inputs.Move;
        //moveAction.performed += OnMove;
        lookAction = inputs.Look;
        inputs.Jump.performed += OnJump;
        inputs.Fish.performed += StartFishing;
        characterController = this.GetComponent<CharacterController>();
        inputs.Interact.performed += OnInteract;
    }
    private void OnEnable()
    {
        //inputs.Player.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        Station.StationInteracted += StationInteracted;
        Station.LeftStation += StationLeft;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool InteractionCheck(out IInteractable interactable)
    {
        var colliders = Physics.OverlapSphere(this.transform.position, 2, interactionLayer);
        foreach (var collider in colliders)
        {
            interactable = collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                return true;
            }
        }
        interactable = null;
        return false;
        // return interactable != null? true: false;
    }
    void OnInteract(InputAction.CallbackContext context)
    {
        IInteractable interactable;
        if (InteractionCheck(out interactable))
        {
            interactable.Interact();

            InputManager.DisablePlayer();
            inStation = true;
        }

    }

    private void StationInteracted(Station station, Transform transform)
    {
        inStation = true;
        this.transform.position = transform.position;
        this.transform.rotation = transform.rotation;
    }
    void StationLeft(Station station)
    {
        inStation = false;
        InputManager.EnablePlayer();


    }
    // Update is called once per frame
    void Update()
    {

        if (inStation)
        {
            InteractionUI.text = "";
            return;
        }
        rotVelocity = Vector2.MoveTowards(rotVelocity, lookAction.ReadValue<Vector2>() * mouseSensitiviy, 0.5f);
        cameraRig.Rotate(new Vector3(-rotVelocity.y, rotVelocity.x, 0));
        var angles = cameraRig.localEulerAngles;
        angles.z = 0;
        var angle = cameraRig.localEulerAngles.x;
        angles.x = ClampRotation(angle, minPitch, maxPitch);
        //print(angles.x);
        cameraRig.localEulerAngles = angles;
        IInteractable interactible;
        if (InteractionCheck(out interactible))
        {
            InteractionUI.text = interactible.StationName();
        }
        else
        {
            if (InteractionUI != null)
            {
                InteractionUI.text = "";

            }

        }

    }
    private void FixedUpdate()
    {
        if (inStation)
        {
            return;
        }
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        velocity = Vector3.MoveTowards(velocity, this.transform.TransformDirection(moveDir) * moveSpeed, (characterController.isGrounded ? accel : accel / 4));
        if (moveAction.IsPressed())
        {
            var angles = cameraRig.localEulerAngles;
            this.transform.Rotate(0, angles.y, 0);
            angles.y = 0;
            cameraRig.localEulerAngles = angles;
        }
        characterController.Move((velocity + (Vector3.up * fallSpeed)) * Time.fixedDeltaTime);

        if (!characterController.isGrounded)
        {
            fallSpeed += Gravity * Time.fixedDeltaTime;
        }
        else
        {
            fallSpeed = -9.8f;
        }
    }

    float ClampRotation(float angle, float minAngle, float maxAngle)
    {
        //print(angle);
        angle %= 360;
        //print(angle);
        //angle=angle - 180f * Mathf.Floor((angle + 180f) / 180f);
        minAngle += 360;
        if (angle > 180 && angle < minAngle % 360)
        {

            angle = minAngle % 360;
        }
        else if (angle < 180 && angle > maxAngle)
        {
            angle = maxAngle;
        }
        return angle;
    }
    private void OnMove(InputAction.CallbackContext context)
    {

    }
    void OnJump(InputAction.CallbackContext context)
    {
        if (!characterController.isGrounded)
        {
            return;
        }
        fallSpeed = Mathf.Sqrt(jumpStrength * -1.0f * Gravity);
    }

    void StartFishing(InputAction.CallbackContext context)
    {
        fishingRod.CastLine(cameraRig.forward);
    }
}
