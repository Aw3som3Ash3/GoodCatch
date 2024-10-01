using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWaterPhysics : WaterPhysics
{
    [SerializeField]
    Transform waterSimulationSimPointGroup;
    List<Transform> waterSimulationPoints;
    Dictionary<Transform, float> waterForceAtPoint;
    // Start is called before the first frame update
    void Start()
    {
        waterForceAtPoint = new Dictionary<Transform, float>();
        //waterSimulationPoints=new Transform[waterSimulationSimPointGroup.childCount];\
        waterSimulationPoints = new List<Transform>();

        foreach (Transform child in waterSimulationSimPointGroup)
        {
            waterSimulationPoints.Add(child);
            waterForceAtPoint[child] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void ApplyWaterForce()
    {
        if (!inWater) { return; }
       
        foreach (Transform t in waterSimulationPoints)
        {
            float targetHeight = waterHeight + waterSimulator.WaterWave(t.position);
            float distance = targetHeight - t.position.y;
            waterForceAtPoint[t] = -Physics.gravity.y * Mathf.Clamp(bouyancy *distance,0,float.MaxValue);
            Debug.DrawLine(t.position, t.position+ waterForceAtPoint[t] * Vector3.up, waterForceAtPoint[t] >= 0 ? Color.cyan : Color.magenta);
            Debug.DrawLine(t.position,new Vector3(t.position.x,targetHeight,t.position.z) , distance > 0?  Color.blue: Color.red);
           
            rb.AddForceAtPosition(waterForceAtPoint[t] * Vector3.up, t.position, ForceMode.Acceleration);
           
        }

    }
}
