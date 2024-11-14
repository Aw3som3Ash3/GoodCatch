using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    protected Rigidbody rb;
    [SerializeField]
    [Range(0, 1)]
    protected float bouyancy;
    [SerializeField]
    protected float dampenForce;
    [SerializeField]
    protected float waterHeight;
   
    protected float velocity;
    protected bool inWater;
    protected WaterSimulator waterSimulator;
    [SerializeField]
    protected float smoothTime;
    float lastHitDistance;
    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        ApplyWaterForce();
    }
    public virtual void EnterWater(float waterHeight, WaterSimulator waterSimulator)
    {
        inWater = true;
        this.waterHeight = waterHeight;
        this.waterSimulator = waterSimulator;
        print(this + " entered water");
    }

    protected virtual void ApplyWaterForce()
    {
        if (!inWater) { return; }
        float targetHeight = waterHeight + waterSimulator.WaterWave(this.transform.position);
        float distance = targetHeight - this.transform.position.y;
        float waterForce = WaterForce(distance)+ dampenForce* lastHitDistance;
        lastHitDistance= distance;
        Debug.DrawLine(this.transform.position, this.transform.position + waterForce * Vector3.up, waterForce >= 0 ? Color.cyan : Color.magenta);
        Debug.DrawLine(this.transform.position, new Vector3(this.transform.position.x, targetHeight, this.transform.position.z), distance > 0 ? Color.blue : Color.red);
        rb.AddForce(waterForce * Vector3.up, ForceMode.Acceleration);


    }

    protected float WaterForce(float distance)
    {
        float force= -Physics.gravity.y * bouyancy * distance;
        return force;

    }

}
