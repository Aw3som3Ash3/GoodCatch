using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //CharacterController characterController;
    Rigidbody rb;
    [SerializeField]
    CinemachineVirtualCamera cameraObj;
    [SerializeField]
    float speed = 5,accel,jumpStrength;
    //Vector3 moveDir;
    Vector3 inputDir;
    const float gravity=-9.8f;
    float fallSpeed;
    Vector3 velocity;
    bool isGrounded;
    bool inWater;
    [SerializeField]
    LayerMask groundLayers;
    private void Awake()
    {
        //characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (!inWater)
        {
            Vector3 moveDir = Quaternion.AngleAxis(cameraObj.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value, Vector3.up) *this.transform.TransformDirection(inputDir);
            velocity = Vector3.MoveTowards(velocity, moveDir * speed, (isGrounded ? accel : inputDir.magnitude * accel / 10));
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }else if (inWater)
        {
            Vector3 moveDir = Camera.main.transform.TransformDirection(inputDir);
            rb.velocity = moveDir*speed;
        }
        if (GroundCheck())
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }


    }
    public void Input(Vector3 moveDir)
    {
        this.inputDir =moveDir;
    }
    public void Jump()
    {
       
        //fallSpeed = Mathf.Sqrt(jumpStrength * -1.0f * gravity);
        if (inWater||!isGrounded)
        {
            return;
        }
        isGrounded = false;
        rb.AddForce(Mathf.Sqrt(jumpStrength * -1.0f * gravity)*Vector3.up, ForceMode.Impulse);
    }
    //public void Camera(Vector2 input)
    //{
    //    //this.transform.Rotate(Vector3.up * input.x);
    //   // cameraObj.Rotate(Vector3.right * -input.y);
    //}
    private void OnCollisionEnter(Collision collision)
    {

        GroundCheck();


    }
    //private void OnCollisionExit(Collision collision) 
    //{
    //    GroundCheck();
    //}
    bool GroundCheck()
    {

        RaycastHit hit;
        if (Physics.SphereCast(this.transform.position, 0.4f, Vector3.down, out hit,0.6f,groundLayers))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        return isGrounded;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<WaterSimulator>())
        {
            if(other.GetComponent<WaterSimulator>().SineWave(this.transform.position) > this.transform.position.y)
            {
                rb.useGravity = false;
                isGrounded=false;
                inWater=true;
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.useGravity = true;
                
                inWater = false;
            }
           
        }
    }
    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    hit.gameObject.GetComponentInParent<ShipSimulator>()?.AddObject(this.transform);
    //}
}
