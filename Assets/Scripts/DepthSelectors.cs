using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSelectors : MonoBehaviour
{
    [SerializeField]
    Depth currentDepth;
    [SerializeField]
    GameObject visualizer,targetingMarker;
    [SerializeField]
    Color color, hoverColor;
    public Depth CurrentDepth { get { return currentDepth; } }
    public Action Selected;

    [SerializeField]
    Transform playerSide, enemySide;
    bool selectorEnabled;
    // Start is called before the first frame update
    void Start()
    {

        visualizer.SetActive(false);
        targetingMarker.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetSelection(bool b)
    {
        selectorEnabled = b;
        visualizer.SetActive(b);

    }

    public void SelectDepth()
    {
        if (!selectorEnabled)
        {
            return;
        }
        targetingMarker.SetActive(false);
        Selected?.Invoke();
        
    }
    public void OnHover(bool b)
    {
        if (selectorEnabled)
        {

            targetingMarker.SetActive(b);
            visualizer.GetComponent<Renderer>().material.color = b ? hoverColor : color;
        }
       


    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(playerSide.position, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(enemySide.position, Vector3.one);
    }
#endif
}
