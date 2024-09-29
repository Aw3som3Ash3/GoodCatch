using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIcon : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    Image border;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIcon(Sprite sprite,Color color)
    {
        icon.sprite=sprite;
        border.color=color;
    }
}
