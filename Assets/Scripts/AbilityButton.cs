using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    TextMeshProUGUI textMesh;
    [SerializeField]
    int index;
    [SerializeField]
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetIndex(int index)
    {
        this.index = index;
    }
    public void UpdateVisuals(string name,Sprite sprite)
    {
        //this.icon.sprite = sprite;
        textMesh.text = name;
    }

    public void Subscribe(Action<int> action)
    {
        print(action);
        button.onClick.AddListener(() => action(index));
    }
}
