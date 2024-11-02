using System.Collections.Generic;
using UnityEngine;

public class ShipWaterPhysics : WaterPhysics
{
    [SerializeField]
    Transform waterSimulationSimPointGroup;
    List<Transform> waterSimulationPoints= new List<Transform>();
    Dictionary<Transform, float> lastHitDistance=new Dictionary<Transform, float>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in waterSimulationSimPointGroup)
        {
            waterSimulationPoints.Add(child);
            lastHitDistance[child] = 0;
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
            float waterForce = WaterForce(distance) + dampenForce * lastHitDistance[t];
            lastHitDistance[t] = distance;
            Debug.DrawLine(t.position, t.position + waterForce * Vector3.up, waterForce >= 0 ? Color.cyan : Color.magenta);
            Debug.DrawLine(t.position, new Vector3(t.position.x, targetHeight, t.position.z), distance > 0 ? Color.blue : Color.red);

            rb.AddForceAtPosition(waterForce * Vector3.up, t.position, ForceMode.Acceleration);

        }

    }
}
