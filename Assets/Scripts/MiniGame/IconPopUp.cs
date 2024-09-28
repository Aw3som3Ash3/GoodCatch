using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerUI : MonoBehaviour
{
    public GameObject UI;

    void OnTriggerEnter(Collider Obj)
    {
        Debug.Log("Haappy");
        if (Obj.tag == "Player")
        {
            UI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider Obj)
    {
        if (Obj.tag == "Player")
        {
            UI.SetActive(false);
        }
    }
}
