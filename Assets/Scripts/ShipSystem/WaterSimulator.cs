using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaterSimulator : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField]
    float waveHeightX,waveFrequencyX,waveSpeedX;
    [SerializeField]
    float waveHeightZ,waveFrequencyZ,waveSpeedZ;
    float timer;
    Vector3 originOffset;
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer=GetComponent<MeshRenderer>();
        FloatingOrigin.OriginShift += OnOriginShift;
        mesh=GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        //print(SineWave(Vector3.zero));
        timer += Time.deltaTime;
        timer %= 200*Mathf.PI;

        meshRenderer.material.SetFloat("_speedX", waveSpeedX);
        meshRenderer.material.SetFloat("_frequencyX", waveFrequencyX);
        meshRenderer.material.SetFloat("_amplitudeX", waveHeightX);
        meshRenderer.material.SetFloat("_speedZ", waveSpeedZ);
        meshRenderer.material.SetFloat("_frequencyZ", waveFrequencyZ);
        meshRenderer.material.SetFloat("_amplitudeZ", waveHeightZ);
        meshRenderer.material.SetFloat("_time", -timer);
        meshRenderer.material.SetVector("_Offset",this.transform.position+originOffset);

        
    }
    private void OnTriggerStay(Collider other)
    {
        //other.GetComponentInParent<WaterPhysics>()?.ApplyWaterForce(this.transform.position.y+SineWave(other.transform.position));
    }
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentInParent<WaterPhysics>()?.EnterWater(this.transform.position.y,this);
    }

    public float SineWave(Vector3 position)
    {
        //return Mathf.Clamp(Mathf.Sin(timer + position.z),0,1) * waveHeight;

        return (Mathf.Sin(((position.z+originOffset.z) * waveFrequencyZ) - timer*waveSpeedZ) * waveHeightZ) + Mathf.Sin(((position.x + originOffset.x) * waveFrequencyX) - timer * waveSpeedX)*waveHeightZ;
    }
    void OnOriginShift(Vector3 position)
    {
        originOffset += position;
    }
}
