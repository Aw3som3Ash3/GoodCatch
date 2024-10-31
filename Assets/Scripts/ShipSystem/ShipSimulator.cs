using UnityEngine;

public class ShipSimulator : MonoBehaviour
{
    [SerializeField]
    Rigidbody physicSim;
    [SerializeField]
    Sail sail;
    [SerializeField]
    Transform childrenObject;
    [SerializeField]
    float maxSpeed, turningSpeed;
    public float sailRatio { get; private set; }
    public float turnRatio { get; private set; }


    public Vector3 Velocity { get { return physicSim.velocity; } }

    // Start is called before the first frame update
    void Start()
    {

        //sailRatio = 1;
        sail.SetrWindStrength(1);
        sail.SetSailsAmount(sailRatio);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {

        physicSim.AddRelativeForce(Vector3.forward * (maxSpeed * sailRatio), ForceMode.Acceleration);
        physicSim.AddRelativeTorque(Vector3.up * turningSpeed * turnRatio, ForceMode.Acceleration);
        this.transform.position = physicSim.transform.position;
        this.transform.rotation = physicSim.transform.rotation;
        physicSim.transform.localPosition = Vector3.zero;
        physicSim.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void AdjustSails(float adjustment)
    {

        //sailRatio = Mathf.Clamp(sailRatio + adjustment , 0, 1);
        sailRatio = adjustment;
        sail.SetSailsAmount(sailRatio);


    }
    public void AdjustTurn(float adjustment)
    {
        // turnRatio= Mathf.Clamp(turnRatio + adjustment, -1, 1);
        turnRatio = adjustment;
    }
    private void LateUpdate()
    {

    }
    public void AddObject(Transform obj)
    {
        obj.SetParent(childrenObject);
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
