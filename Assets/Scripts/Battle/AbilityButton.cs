using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public Action<int> OnHover,OnHoverExit;

// Start is called before the first frame update
    void Start()
    {
        EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
        hoverEvent.eventID = EventTriggerType.PointerEnter;
        hoverEvent.callback.AddListener((eventData) => { if (isActiveAndEnabled) { OnHover?.Invoke(index); } });

        EventTrigger.Entry exitEvent = new EventTrigger.Entry();
        exitEvent.eventID = EventTriggerType.PointerExit;
        exitEvent.callback.AddListener((eventData) => { if (isActiveAndEnabled) { OnHoverExit?.Invoke(index);} });

        button.AddComponent<EventTrigger>().triggers.Add(hoverEvent);
        button.GetComponent<EventTrigger>().triggers.Add(exitEvent);
        

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
        button.onClick.AddListener(() => { if (isActiveAndEnabled) { action(index); } });
    }
    private void OnEnable()
    {
        button.enabled = true;
        icon.color= Color.white;
    }
    private void OnDisable()
    {
        button.enabled = false;
        icon.color = Color.gray;
    }

}
