using UnityEngine;

public class SimpleWaterPhysics : WaterPhysics
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void EnterWater(float waterHeight, WaterSimulator waterSimulator)
    {
        base.EnterWater(waterHeight, waterSimulator);
        rb.isKinematic = true;
    }
    protected override void ApplyWaterForce()
    {
        if (!inWater) { return; }

        float targetHeight = waterHeight + waterSimulator.WaterWave(this.transform.position);
        //print("target height "+ targetHeight+ " sinewave: " + waterSimulator.SineWave(this.transform.position));
        this.transform.position = new Vector3(this.transform.position.x, targetHeight, this.transform.position.z);
    }
}
