using System;
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
    [SerializeField]
    Image[] useDepthsIcons;
    [SerializeField]
    Image[] attackDepthsIcons;

    [Header("Tooltip")]
    [SerializeField]
    GameObject toolTip;
    [SerializeField]
    TextMeshProUGUI toolTipAbilityName,textArea;
    public Action<int> OnHover, OnHoverExit;


    // Start is called before the first frame update
    void Start()
    {
        toolTip.SetActive(false);
        EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
        hoverEvent.eventID = EventTriggerType.PointerEnter;
        hoverEvent.callback.AddListener(OnPointerEnter);

        EventTrigger.Entry exitEvent = new EventTrigger.Entry();
        exitEvent.eventID = EventTriggerType.PointerExit;
        exitEvent.callback.AddListener(OnPointerExit);

        button.AddComponent<EventTrigger>().triggers.Add(hoverEvent);
        button.GetComponent<EventTrigger>().triggers.Add(exitEvent);


    }
    void OnPointerEnter(BaseEventData eventData)
    {
        if (!isActiveAndEnabled) 
        {
            return;
        }
        toolTip.SetActive(true);
        OnHover?.Invoke(index);
    }
    void OnPointerExit(BaseEventData eventData)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        toolTip.SetActive(false);
        OnHoverExit?.Invoke(index);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SetIndex(int index)
    {
        this.index = index;
    }
    public void UpdateVisuals(Ability ability,float modifiedDamage=0)
    {
        //this.icon.sprite = sprite;
        textMesh.text = ability.name;
        toolTipAbilityName.text = ability.name;
        string text="";
        if (modifiedDamage > 0)
        {
            text += "<color=red> DAMAGE</color>";
        }
        //var checkTypeValues = Enum.GetValues(typeof(Depth));
        for (int i = 0; i< 3; i++)
        {
            
            if(ability.AvailableDepths.HasFlag((Depth)(1 << i)))
            {
                useDepthsIcons[i].color = Color.yellow;
            }
            else
            {
                useDepthsIcons[i].color = Color.gray;
            }
            if (ability.TargetableDepths.HasFlag((Depth)(1 << i)))
            {
                attackDepthsIcons[i].color = ability.TargetedTeam == Ability.TargetTeam.enemy? Color.red:Color.green;
            }
            else
            {
                attackDepthsIcons[i].color = Color.gray;
            }
        }
       
    }

    public void Subscribe(Action<int> action)
    {
        print(action);
        button.onClick.AddListener(() => { if (isActiveAndEnabled) { action(index); } });
    }
    private void OnEnable()
    {
        button.enabled = true;
        icon.color = Color.white;
    }
    private void OnDisable()
    {
        button.enabled = false;
        icon.color = Color.gray;
    }

}
