using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail : MonoBehaviour
{
    MeshRenderer meshRenderer;
    float windStrength;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //
        
    }

    public void SetSailsAmount(float x)
    {
        meshRenderer.material.SetFloat("_Extended", x);
    }
    public void SetrWindStrength(float x)
    {
        windStrength = x;
        meshRenderer.material.SetFloat("_Wind", windStrength);
    }
}
