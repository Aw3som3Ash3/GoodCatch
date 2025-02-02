using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour,ISaveable
{
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
    LayerMask interactionLayer;

    AudioController audioController;
    [SerializeField]
    FishingRod fishingRod;
    Label InteractionUI;
    Animator anim;

    bool inStation;

    bool sprinting { get { return inputs.Sprint.IsPressed(); } }

    string id="player";
    public string ID => id;

    object ISaveable.DataToSave {get{ return new SaveData(Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale), Matrix4x4.TRS(model.transform.position, model.transform.rotation, model.transform.localScale)); } }
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
        inputs = InputManager.Input.Player;
        InputManager.EnablePlayer();
        moveAction = inputs.Move;
        //moveAction.performed += OnMove;
        lookAction = inputs.Look;
        inputs.Jump.performed += OnJump;
        inputs.Fish.performed += StartFishing;
        characterController = this.GetComponent<CharacterController>();
        inputs.Interact.performed += OnInteract;
        anim = GetComponentInChildren<Animator>();
        fishingRod.gameObject.SetActive(false);
        audioController=GetComponent<AudioController>();
        //InteractionUI = FindObjectOfType<UIDocument>().rootVisualElement.Q<Label>("InteractionHud");
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
            if(interactable is Station)
            {
                InputManager.DisablePlayer();
                inStation = true;
            }
           
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
            InteractionUI.text = interactible.StationName;
        }
        else
        {
            if (InteractionUI == null)
            {
                InteractionUI = FindObjectOfType<UIDocument>().rootVisualElement.Q<Label>("InteractionHud");
            }
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
        velocity = Vector3.MoveTowards(velocity, this.transform.TransformDirection(moveDir) * (sprinting?sprintSpeed: moveSpeed) , (characterController.isGrounded ? accel : accel / 4));
        if (moveAction.IsInProgress())
        {
            
            var angles = cameraRig.localEulerAngles;
            this.transform.Rotate(0, angles.y, 0);
            angles.y = 0;
            cameraRig.localEulerAngles = angles;
            var targetRot= Quaternion.LookRotation(this.transform.TransformDirection(moveDir.normalized));
            model.rotation = Quaternion.RotateTowards(model.rotation, targetRot, 720*Time.deltaTime);
        }
        anim.SetFloat("speed",velocity.magnitude);
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
}
