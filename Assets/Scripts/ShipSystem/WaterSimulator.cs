using System;
using UnityEngine;

public class WaterSimulator : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField]
    float waveHeightX, waveFrequencyX, waveSpeedX;
    [SerializeField]
    float waveHeightZ, waveFrequencyZ, waveSpeedZ;
    [SerializeField]
    bool useTide;
    [SerializeField]
    float tideMultiplier;
    float timer;
    Vector3 originOffset;
    Mesh mesh;
    WaterSimulator parent;
    float tideLevel;

    [SerializeField]
    LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        FloatingOrigin.OriginShift += OnOriginShift;
        mesh = GetComponent<MeshFilter>().mesh;
        parent= this.transform.parent?.GetComponent<WaterSimulator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(SineWave(Vector3.zero));
        //timer += Time.deltaTime;
        
        //timer %= 200*Mathf.PI;
        if (parent != null)
        {
            waveSpeedX = parent.waveSpeedX;
            waveHeightX = parent.waveHeightX;
            waveHeightZ = parent.waveHeightZ;
            waveSpeedZ = parent.waveSpeedZ;
            waveFrequencyZ = parent.waveFrequencyZ;
            waveFrequencyX=parent.waveFrequencyX;
            timer = parent.timer;
            useTide = parent.useTide;
        }
        else
        {
            timer = Time.time;
            if (useTide)
            {
                tideLevel = Mathf.Sin(GameManager.Instance.DayTime * (2 * Mathf.PI / 12))*tideMultiplier;
            }
        }

        RaycastHit hit;
        float modWaveSpeedX = waveSpeedX;
        float modWaveHeightX = waveHeightX;
        float modWaveHeightZ = waveHeightZ;
        float modWaveSpeedZ = waveSpeedZ;
        float modWaveFrequencyZ = waveFrequencyZ;
        float modWaveFrequencyX = waveFrequencyX;

        if (Physics.Raycast(this.transform.position + Vector3.up * 100, Vector3.down, 105,groundLayer))
        {
            Debug.Log("dry land");
        }
        else 
        {
            if (Physics.Raycast(this.transform.position, Vector3.down, out hit))
            {
                Debug.Log("deepish water");
                //modWaveSpeedX += waveSpeedX;
                modWaveHeightX += hit.distance / 50;
                modWaveHeightZ += hit.distance / 50;
                //modWaveSpeedX += waveSpeedZ;
                //modWaveSpeedX += waveFrequencyZ;
                //modWaveSpeedX += waveFrequencyX;

            }
            else
            {
                Debug.Log("deep");
                modWaveHeightX *= 1.5f;
                modWaveHeightZ *= 1.5f;
            }
        }


        meshRenderer.material.SetFloat("_speedX", modWaveSpeedX);
        meshRenderer.material.SetFloat("_frequencyX", modWaveFrequencyX);
        meshRenderer.material.SetFloat("_amplitudeX", modWaveHeightX);
        meshRenderer.material.SetFloat("_speedZ", modWaveSpeedZ);
        meshRenderer.material.SetFloat("_frequencyZ", modWaveFrequencyZ);
        meshRenderer.material.SetFloat("_amplitudeZ", modWaveHeightZ);
        meshRenderer.material.SetFloat("_time", timer);
        meshRenderer.material.SetVector("_Offset", originOffset+Vector3.up*tideLevel);
    }
    
    private void OnTriggerStay(Collider other)
    {
        //other.GetComponentInParent<WaterPhysics>()?.ApplyWaterForce(this.transform.position.y+SineWave(other.transform.position));
    }
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentInParent<WaterPhysics>()?.EnterWater(this.transform.position.y, this);
    }

    public float WaterWave(Vector3 position)
    {
        //return Mathf.Clamp(Mathf.Sin(timer + position.z),0,1) * waveHeight;
        float sineWaveZ = SineWave(position.z + originOffset.z, waveFrequencyZ, waveSpeedZ, waveHeightZ);
        float sineWaveX = SineWave(position.x + originOffset.x, waveFrequencyX, waveSpeedX, waveHeightX);
        return -(sineWaveZ + sineWaveX);
    }
    float SineWave(float position, float waveFrequency, float speed, float waveHeight)
    {
        return (Mathf.Sin((position * waveFrequency) - timer * speed) * waveHeight)+tideLevel;
    }
    void OnOriginShift(Vector3 position)
    {
        originOffset += position;
    }
}
