using System;
using UnityEngine;

public class ShipSimulator : MonoBehaviour,ISaveable
{
    [SerializeField]
    Rigidbody physicSim;
    public Rigidbody PhysicSim { get { return physicSim; } }
    //[SerializeField]
    //Sail sail;
    [SerializeField]
    Transform childrenObject;
    [SerializeField]
    float maxSpeed, turningSpeed;
    public float sailRatio { get; private set; }
    public float turnRatio { get; private set; }

    public AudioController audioController;
    public AudioSource audioSource;
    private AudioClip activeClip;

    [SerializeField]
    Transform wheelRight, wheelLeft;
    public Vector3 Velocity { get { return physicSim.velocity; } }

    public object DataToSave => Matrix4x4.TRS(this.transform.position,this.transform.rotation,this.transform.localScale);

    public string ID => "ship";



    [SerializeField]
    PIDController zTorqueController=new (0.5f,0,0.25f);
    [SerializeField]
    PIDController xTorqueController=new(0.5f,0,0.25f);

    [SerializeField]
    Transform repawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        
        //sailRatio = 1;
        //sail.SetrWindStrength(1);
        //sail.SetSailsAmount(sailRatio);
    }

    // Update is called once per frame
    void Update()
    {
        //wheelLeft.transform.Rotate(Vector3.right*)
    }
    private void FixedUpdate()
    {

        physicSim.AddRelativeForce(Vector3.forward * (maxSpeed * sailRatio- turnRatio / 2), ForceMode.Acceleration);
        //physicSim.maxLinearVelocity = maxSpeed * (1-turnRatio / 2);
        physicSim.AddRelativeTorque(Vector3.up * turningSpeed * turnRatio, ForceMode.Acceleration);
        this.transform.position = physicSim.transform.position;
        this.transform.rotation = physicSim.transform.rotation;
        physicSim.transform.localPosition = Vector3.zero;
        physicSim.transform.localRotation = Quaternion.Euler(Vector3.zero);
        wheelLeft.Rotate(Vector3.right * Mathf.Clamp(sailRatio - turnRatio / 2, -1, 1) * maxSpeed*5 * Time.fixedDeltaTime);
        wheelRight.Rotate(Vector3.right * Mathf.Clamp(sailRatio + turnRatio/2,-1,1) * maxSpeed*5 * Time.fixedDeltaTime);

        physicSim.AddRelativeTorque(Vector3.forward*zTorqueController.PID(Time.fixedDeltaTime, TurnAngleToSignedAngle(this.transform.localEulerAngles.z), 0), ForceMode.Acceleration);
        physicSim.AddRelativeTorque(Vector3.right*xTorqueController.PID(Time.fixedDeltaTime, TurnAngleToSignedAngle(this.transform.localEulerAngles.x), 0),ForceMode.Acceleration);
        
    }
    public Vector3 SafePosition()
    {
        return repawnPoint.position;
    }
    public void AnchorShip()
    {
        physicSim.maxLinearVelocity = 0;
    }
    public void UnAnchorShip()
    {
        physicSim.maxLinearVelocity = maxSpeed*4;
    }

    public float TurnAngleToSignedAngle(float angle)
    {
        // Normalize the angle to be within 0 to 360 degrees
        angle = angle % 360;

        // Convert to signed angle
        if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }
    public void AdjustSails(float adjustment)
    {

        sailRatio = Mathf.Clamp(sailRatio + adjustment , 0, 1);
        sailRatio = adjustment;
       

        if (sailRatio > 0)
        {
            audioSource.volume = sailRatio;
            audioSource.clip = audioController.clip[0];
            audioSource.Play();
        }
        else
        {
            audioSource.volume = 0.3f;
            audioSource.clip = audioController.clip[1];
            audioSource.Play();
        }
        
    }
    public void AdjustTurn(float adjustment)
    {
        turnRatio= Mathf.Clamp(turnRatio + adjustment, -1, 1);
        turnRatio = adjustment;
    }
    private void LateUpdate()
    {

    }
    public void AddObject(Transform obj)
    {
        obj.SetParent(childrenObject);
    }

    public void Load(string json)
    {

        Matrix4x4 data = JsonUtility.FromJson<Matrix4x4>(json);

        this.transform.position = data.GetPosition();
        this.transform.rotation = data.rotation;
        this.transform.localScale = data.lossyScale;

    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    print("object in trigger");
    //    AddObject(other.transform);
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    other.transform.parent = null;
    //}
}

[Serializable]
public class PIDController
{
    public float proportionalGain=1;
    public float intergralGain;
    public float derivativeGain=0.5f;
    public float intergralSaturation;
    public float maxOutput=1;


    float valueLast;

    bool derrivativeInitialized = false;

    float intergrationStored;
    public PIDController()
    {

    }
    public PIDController(float proportionalGain, float inergralGain,float derivativeGain )
    {
        this.proportionalGain= proportionalGain;
        this.intergralGain= inergralGain;
        this.derivativeGain= derivativeGain;
    }
    public float TorquePID(float dt, float currentAngle, float targetAngle)
    {
       

        float error =   AngleDifference(currentAngle, targetAngle);



        float P = proportionalGain * error;
        float valueRateOfChange = AngleDifference(currentAngle, valueLast) / dt;
        valueLast = currentAngle;
        float deriveMeasure = 0;
       
        if (derrivativeInitialized)
        {
          
            deriveMeasure = -valueRateOfChange;
        }
        else 
        {
            derrivativeInitialized = true;
        }

        intergrationStored = Mathf.Clamp(intergrationStored + (error * dt), -intergralSaturation, intergralSaturation);
        float I = intergralGain * intergrationStored;


        float D=derivativeGain * deriveMeasure;
        float result = P +I+ D;


        return Mathf.Clamp(result, -maxOutput, maxOutput);
    }

    float AngleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;
    }
    public float PID(float dt, float currentValue, float targetValue)
    {


        float error = targetValue - currentValue;



        float P = proportionalGain * error;
        float valueRateOfChange = (currentValue - valueLast) / dt;
        valueLast=currentValue;
        float deriveMeasure = 0;

        if (derrivativeInitialized)
        {

            deriveMeasure = -valueRateOfChange;
        }
        else
        {
            derrivativeInitialized = true;
        }

        intergrationStored = Mathf.Clamp(intergrationStored + (error * dt), -intergralSaturation, intergralSaturation);
        float I = intergralGain * intergrationStored;


        float D = derivativeGain * deriveMeasure;
        float result = P + I + D;


        return result;
    }
    public void Reset()
    {
        derrivativeInitialized = false;
    }
}