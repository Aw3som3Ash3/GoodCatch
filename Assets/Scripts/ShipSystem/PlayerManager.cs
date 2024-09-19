using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{

    normal,
    stationed,
    dead
}
public class PlayerManager : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    Vector3 moveDir;
    [SerializeField]
    PlayerController playerController;
    PlayerState state;
    [SerializeField]
    float cameraSentisitivity;
    Station currentStation;
    Camera currentCamera;
    [SerializeField]
    LayerMask interactLayers;
    Transform prevParent;
    [SerializeField]
    GameObject pScreen, oScreen;
    [SerializeField]
    TextMeshProUGUI interactUI;
    [SerializeField]
    Transform resetPosition;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
       
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        playerInput.actions["P"].performed+=P;
        playerInput.actions["O"].performed += O;
        playerInput.actions["Pause"].performed += PauseGame;
        playerInput.actions["Interact"].performed += OnInteract;
        playerInput.actions["Fire"].performed += Fire;
        playerInput.actions["Reset"].performed += ResetCharacter;
        
        Station.StationInteracted += StationInteracted;
        currentCamera = Camera.main;
        pScreen.SetActive(false);
        oScreen.SetActive(false);
       
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(PlayerState.normal);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 moveInput = moveAction.ReadValue<Vector2>();
    }
    void P(InputAction.CallbackContext input)
    {
        pScreen.SetActive(!pScreen.activeSelf);
    }
    void O(InputAction.CallbackContext input)
    {
        oScreen.SetActive(!oScreen.activeSelf);
    }
    private void FixedUpdate()
    {

        RaycastHit hit;
        if (Physics.Raycast(currentCamera.transform.position, currentCamera.transform.forward, out hit, 1.5f, interactLayers)&&state==PlayerState.normal)
        {
            if (hit.collider.GetComponentInParent<IInteractable>()?.Interactable()==true)
            {
                interactUI.text=hit.collider.GetComponentInParent<IInteractable>().StationName();
            }
            else
            {
                interactUI.text = "";
            }

        }
        else
        {
            interactUI.text = "";
        }
    }
    public void StationInteracted(Station station,Transform stationZone)
    {
        this.currentStation = station;
        ChangeState(PlayerState.stationed);
        prevParent = playerController.transform.parent;
        playerController.transform.parent = stationZone;
        playerController.transform.localPosition = Vector3.zero;

    }
    void MoveAction(InputAction.CallbackContext input)
    {
        Vector2 moveInput = input.ReadValue<Vector2>();
        if (state == PlayerState.normal)
        {
            playerController.Input(new Vector3(moveInput.x, 0, moveInput.y));
        }else if(state == PlayerState.stationed)
        {
            currentStation.MoveInput(moveInput);
        }
       
    }
    void LookAction(InputAction.CallbackContext input)
    {
        //playerController.Camera(input.ReadValue<Vector2>()* cameraSentisitivity);
    }
    void JumpAction(InputAction.CallbackContext input)
    {
        playerController.Jump();
    }
    void ChangeState(PlayerState state)
    {
        this.state = state;
        switch (state) 
        {
            case PlayerState.normal:
                moveAction.performed += MoveAction;
                moveAction.canceled += MoveAction;
                lookAction.performed += LookAction;
                jumpAction.performed += JumpAction;
                break;
            case PlayerState.stationed:
                //moveAction.performed -= MoveAction;
                //moveAction.canceled -= MoveAction;
                jumpAction.performed -= JumpAction;
                break;
            case PlayerState.dead:
                moveAction.performed -= MoveAction;
                moveAction.canceled -= MoveAction;
                jumpAction.performed -= JumpAction;
                break;
            default:
                break;
        }

    }
    void OnInteract(InputAction.CallbackContext input)
    {
        if (state == PlayerState.stationed)
        {
            currentStation.LeaveSation();
            currentStation = null;
            ChangeState(PlayerState.normal);
            playerController.transform.parent = prevParent;
            return;
        }
        RaycastHit hit;
        if(Physics.Raycast(currentCamera.transform.position, currentCamera.transform.forward,out hit, 1.5f, interactLayers))
        {
            hit.collider.GetComponentInParent<IInteractable>()?.Interact();
        }
    }
   
    void PauseGame(InputAction.CallbackContext input)
    {
        //if (!this.isActiveAndEnabled)
        //{
        //    return;
        //}
        //GameManager.Instance.Pause();
    }
    void Fire(InputAction.CallbackContext input)
    {
        if (state== PlayerState.stationed)
        {
            currentStation.Use();
        }
    }

    void ResetCharacter(InputAction.CallbackContext input)
    {
        playerController.transform.position = resetPosition.position;
    }
}
