using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GoodCatchInputs.PlayerActions inputs;
    InputAction moveAction;
    CharacterController characterController;
    [SerializeField]
    float moveSpeed, accel;
    Vector3 velocity;
    float fallSpeed;
    const float Gravity = -9.8f;
    private void Awake()
    {
        inputs = InputManager.Input.Player;
        InputManager.EnablePlayer();
        moveAction = inputs.Move;
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
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        velocity = Vector3.MoveTowards(velocity, this.transform.TransformDirection(moveDir)* moveSpeed, accel);
        characterController.Move( (velocity + (Vector3.up * fallSpeed))*Time.deltaTime);

        if (!characterController.isGrounded)
        {
            fallSpeed+=Gravity*Time.deltaTime;
        }
        else
        {
            fallSpeed = -0.1f;
        }
    }

   
}
