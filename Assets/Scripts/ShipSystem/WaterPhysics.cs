using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    protected Rigidbody rb;
    [SerializeField][Range(0,1)]
    protected float bouyancy;
    [SerializeField]
    protected float waterHeight;
    float waterForce;
    protected float velocity;
    protected bool inWater;
    protected WaterSimulator waterSimulator;
    [SerializeField]
    protected float smoothTime;
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
    public void EnterWater(float waterHeight, WaterSimulator waterSimulator)
    {
        inWater = true;
        this.waterHeight = waterHeight;
        this.waterSimulator = waterSimulator;
    }
    
    protected virtual void ApplyWaterForce()
    {
        if (!inWater) { return; }
        waterForce = Mathf.Clamp(-Physics.gravity.y * bouyancy *(waterHeight + waterSimulator.SineWave(this.transform.position) - this.transform.position.y),0,float.MaxValue);
        rb.AddForce(waterForce * Vector3.up, ForceMode.Acceleration);
    }

}
