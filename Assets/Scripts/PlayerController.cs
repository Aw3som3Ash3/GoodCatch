using System.Collections;
using System.Collections.Generic;
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
    float moveSpeed, accel;
    [SerializeField]
    float maxPitch, minPitch;
    Vector3 velocity;
    float fallSpeed;
    const float Gravity = -9.8f;
    const float rotSensitivity=10;
    Vector2 rotVelocity;
    private void Awake()
    {
        inputs = InputManager.Input.Player;
        InputManager.EnablePlayer();
        moveAction = inputs.Move;
        moveAction.performed += OnMove;
        lookAction = inputs.Look;
        characterController=this.GetComponent<CharacterController>();   
    }
    private void OnEnable()
    {
        //inputs.Player.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        rotVelocity = Vector2.MoveTowards(rotVelocity, lookAction.ReadValue<Vector2>(), 0.2f);
        cameraRig.Rotate(new Vector3(-rotVelocity.y, rotVelocity.x, 0));
        var angles = cameraRig.localEulerAngles;
        angles.z = 0;
        var angle = cameraRig.localEulerAngles.x;
        angles.x = ClampRotation(angle, minPitch, maxPitch);
        //print(angles.x);
        cameraRig.localEulerAngles = angles;
    }
    private void FixedUpdate()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        velocity = Vector3.MoveTowards(velocity, this.transform.TransformDirection(moveDir) * moveSpeed, accel);
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
            fallSpeed = -0.1f;
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
}
