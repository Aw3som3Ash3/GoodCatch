using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionToken : MonoBehaviour
{
    [SerializeField]
    Image token;
    [SerializeField]
    Color used, left;
    // Start is called before the first frame update
    void Start()
    {
        token.color = left;
    }
    public void Use()
    {
        token.color = used;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
