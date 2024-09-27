using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMouseOver : MonoBehaviour
{
    Color m_MouseOverColor = Color.red;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;

    void Start()
    {  
        m_Renderer = GetComponent<MeshRenderer>();
        m_OriginalColor = m_Renderer.material.color;
    }
    void OnMouseOver()
    {
        m_Renderer.material.color = m_MouseOverColor;
    }

    void OnMouseExit()
    {
        m_Renderer.material.color = m_OriginalColor;
    }
}
