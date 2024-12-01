using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jaylee_testscript_00 : MonoBehaviour
{
    private float i;
    bool b;

    public AudioController audioController;

    // Start is called before the first frame update
    void Start()
    {
        i = 2;
        b = true;
    }

    // Update is called once per frame
    void Update()
    {
        i = i - Time.deltaTime;
        if (i < 0 && b == true)
        {
            
            b = false;
        }
    }
}
