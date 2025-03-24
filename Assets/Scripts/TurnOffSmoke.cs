using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnOffSmoke : MonoBehaviour
{
    [SerializeField]
    ParticleSystem SmokeVFX;
    // Start is called before the first frame update
    void Start()
    {
        SmokeVFX.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        SmokeVFX.Stop();
    }

    private void OnTriggerExit(Collider other)
    {
        SmokeVFX.Play();
    }
}
