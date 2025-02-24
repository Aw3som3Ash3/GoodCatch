using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour,ISaveable
{
   
    static public PlayerController player;
    GoodCatchInputs.PlayerActions inputs;
    InputAction moveAction;
    InputAction lookAction;
    CharacterController characterController;
    [SerializeField]
    Transform cameraRig;
    [SerializeField]
    Transform model;
    [SerializeField]
    float moveSpeed, sprintSpeed,accel, jumpStrength;
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
    LayerMask interactionLayer,waterLayer;

    AudioController audioController;
    [SerializeField]
    FishingRod fishingRod;
    VisualElement InteractionUI;
    Animator anim;

    bool inStation;

    bool sprinting;

    string id="player";
    public string ID => id;
    List<Vector3> lastSafePosition=new();
    object ISaveable.DataToSave {get{ return new SaveData(Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale), Matrix4x4.TRS(model.transform.position, model.transform.rotation, model.transform.localScale)); } }
    [SerializeField]
    ShipSimulator ship;

    [Serializable]
    struct SaveData
    {
        [SerializeField]
        public Matrix4x4 transform,modelTransform;
        public SaveData(Matrix4x4 transform,Matrix4x4 modelTransform)
        {
            this.transform = transform;
            this.modelTransform = modelTransform;
        }
    }

    private void Awake()
    {
        player = this;
        inputs = InputManager.Input.Player;
        InputManager.EnablePlayer();
        moveAction = inputs.Move;
        //moveAction.performed += OnMove;
        lookAction = inputs.Look;
        //lookAction.performed += OnLook;
        //inputs.Jump.performed += OnJump;
        inputs.Fish.performed += StartFishing;
        inputs.Sprint.performed += OnSprint;
        characterController = this.GetComponent<CharacterController>();
        inputs.Interact.performed += OnInteract;
        anim = GetComponentInChildren<Animator>();
        fishingRod.gameObject.SetActive(false);
        audioController=GetComponent<AudioController>();
        //InteractionUI = FindObjectOfType<UIDocument>().rootVisualElement.Q<Label>("InteractionHud");
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        if (context.control.device is Gamepad)
        {
            if (context.action.IsPressed())
            {
                Debug.Log(" toggle sprint");
                sprinting = !sprinting;
            }
            
        }
        else
        {
            sprinting = context.action.IsPressed();
        }
        
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
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        if (InteractionUI == null)
        {
            InteractionUI = GameObject.Find("MainHud").GetComponent<UIDocument>().rootVisualElement.Q("InteractionHud");
        }
    }
    
    bool InteractionCheck(out IInteractable interactable)
    {
        Collider[] colliders= new Collider[10];
        Physics.OverlapSphereNonAlloc(this.transform.position, 2, colliders, interactionLayer);
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
            if(interactable is Station)
            {
                InputManager.DisablePlayer();
                inStation = true;
            }
           
        }

    }
    public void SetPosition(Vector3 pos)
    {
        characterController.enabled = false;
        this.transform.position = pos;
        characterController.enabled = true;
    }
    public void SetPositionAndRotaion(Vector3 pos,Quaternion rotation)
    {
        characterController.enabled = false;
        this.transform.position = pos;
        this.transform.rotation = rotation;
        characterController.enabled = true;
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
            InteractionUI.visible = false;
            InteractionUI.Q<Label>().text = "";
            return;
        }
        OnLook();
        IInteractable interactible;

        if (InteractionUI == null)
        {
            InteractionUI = GameObject.Find("MainHud").GetComponent<UIDocument>().rootVisualElement.Q("InteractionHud");
        }
        if (InteractionCheck(out interactible))
        {
            InteractionUI.visible = true;
            InteractionUI.Q<Label>().text = interactible.StationName;
        }
        else
        {
            
            if (InteractionUI != null)
            {
                InteractionUI.Q<Label>().text = "";
                InteractionUI.visible = false;

            }

        }

    }
    void OnLook()
    {
        rotVelocity = Vector2.MoveTowards(rotVelocity, lookAction.ReadValue<Vector2>() * (InputManager.inputMethod==InputMethod.mouseAndKeyboard? mouseSensitiviy:1), 0.5f);
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
        if (inStation)
        {
            return;
        }
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        if (moveDir.sqrMagnitude.Equals(0))
        {
            sprinting = inputs.Sprint.IsPressed();
        }

        if (moveAction.IsInProgress())
        {

            var angles = cameraRig.localEulerAngles;
            this.transform.Rotate(0, angles.y, 0);
            angles.y = 0;
            cameraRig.localEulerAngles = angles;
            var targetRot = Quaternion.LookRotation(this.transform.TransformDirection(moveDir.normalized));
            model.rotation = Quaternion.RotateTowards(model.rotation, targetRot, 720 * Time.deltaTime);
        }
        velocity = Vector3.MoveTowards(velocity, this.transform.TransformDirection(moveDir) * (sprinting ? sprintSpeed : moveSpeed), (characterController.isGrounded ? accel : accel / 4));
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position+(Vector3.up) + this.transform.TransformDirection(moveDir).normalized, Vector3.down,out hit,100))
        {
            var water = hit.collider.GetComponent<WaterSimulator>();
            if (water != null)
            {
                Vector3 adjustedPoint = hit.point;
                //adjustedPoint.y = water.transform.position.y;
                if (!Physics.Raycast(adjustedPoint, Vector3.down, out hit, 0.2f,~waterLayer))
                {
                    velocity = Vector3.zero;
                }
                
            }
               
        }
      
        
        anim.SetFloat("speed",velocity.magnitude);

        Vector3 shipVelocity=Vector3.zero;
        var ship = GetComponentInParent<ShipSimulator>();
        if (ship!=null)
        {
            shipVelocity = ship.Velocity;
        }
        characterController.Move((velocity + shipVelocity + (Vector3.up * fallSpeed)) * Time.fixedDeltaTime);

        if (!characterController.isGrounded)
        {
            fallSpeed += Gravity * Time.fixedDeltaTime;
        }
        else
        {
            fallSpeed = -9.8f;
            
            lastSafePosition.Add(this.transform.position);
            if (lastSafePosition.Count > 60)
            {
                lastSafePosition.RemoveAt(0);
            }
            
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
        if (context.action.IsPressed())
        {
            Debug.Log("prep to cast");
            //var targetRot = Quaternion.LookRotation(this.transform.TransformDirection(cameraRig.forward));
            model.localEulerAngles = new Vector3(model.localEulerAngles.x, cameraRig.transform.localEulerAngles.y, model.localEulerAngles.z); 
            fishingRod.gameObject.SetActive(true);
            anim.SetTrigger("cast");
            moveAction.Disable();
           
            CancelInvoke("FishingCompleted");
        }
        
       
    }
    void Casted()
    {
        if (inputs.Fish.IsPressed())
        {
            anim.speed = 0;
            Debug.Log("CASTINGG");
            inputs.Fish.canceled += Released;

        }
        else
        {
            inputs.Fish.canceled -= Released;
            anim.SetTrigger("FishingComplete");
            Invoke("FishingCompleted", 1f);
        }
      
        

    }

    private void Released(InputAction.CallbackContext context)
    {
        if (context.duration > 0.5)
        {
            anim.speed = 1;
            fishingRod.CastLine(model.forward,Mathf.Clamp((float)context.duration,0,1.5f), () => { anim.SetTrigger("FishingComplete"); Invoke("FishingCompleted", 1f); });
        }
        
        
        inputs.Fish.canceled -= Released;
        //throw new NotImplementedException();
    }

    void FishingCompleted()
    {
        fishingRod.gameObject.SetActive(false);
        moveAction.Enable();
        
    }

    public void Load(string json)
    {
        SaveData data= JsonUtility.FromJson<SaveData>(json);
        characterController.enabled = false;

        Matrix4x4 matrix = data.transform;
        this.transform.position = matrix.GetPosition();
        this.transform.rotation = matrix.rotation;
        this.transform.localScale = matrix.lossyScale;

        Matrix4x4 modelTransforms = data.modelTransform;
        characterController.enabled = false;
        model.transform.position = modelTransforms.GetPosition();
        model.transform.rotation = modelTransforms.rotation;
        model.transform.localScale = modelTransforms.lossyScale;


        characterController.enabled = true;
    }

    void OnDestroy()
    {
        inputs.Jump.performed -= OnJump;
        inputs.Fish.performed -= StartFishing;
        inputs.Interact.performed -= OnInteract;
        Station.StationInteracted -= StationInteracted;
    }

    void Footstep()
    {
        audioController?.PlayClipRandom();
        //do audio event
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water")&& other.gameObject.GetComponent<WaterSimulator>()!=null)
        {
            characterController.enabled = false;
            if (this.transform.position.y + 1 < other.transform.position.y)
            {
                RaycastHit hit;
                if (Physics.Raycast(lastSafePosition[0], Vector3.down,out hit, 1.2f))
                {
                    if (!hit.collider.CompareTag("Water"))
                    {
                        this.transform.position = lastSafePosition[0];
                    }
                    else
                    {
                        this.transform.position = ship.SafePosition();
                    }
                }
                else
                {
                    this.transform.position = ship.SafePosition();
                }
               
            }
            characterController.enabled = true;

        }
        
    }
}
