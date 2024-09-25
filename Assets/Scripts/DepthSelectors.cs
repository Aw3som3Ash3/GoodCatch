using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSelectors : MonoBehaviour
{
    [SerializeField]
    Depth currentDepth;
    public Action Selected;

    [SerializeField]
    Transform playerSide, enemySide;
    bool selectorEnabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetSelection(bool b)
    {
        selectorEnabled = b;
    }

    public void SelectDepth()
    {
        if (!selectorEnabled)
        {
            return;
        }
        Selected?.Invoke();
    }
}
