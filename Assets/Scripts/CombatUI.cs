using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    Button move,goFirst,ability1,ability2,ability3,ability4;

    public Action MoveAction, GoFirstAction;
    public Action<int> Ability;
    bool isActive;
    [SerializeField]
    RectTransform turnMarker;

    // Start is called before the first frame update
    void Start()
    {
       
        

    }

    public void EnableButtons()
    {

        move.enabled = true;
        goFirst.enabled = true;
        ability1.enabled = true;
        ability2.enabled = true;
        ability3.enabled = true;
        ability4.enabled = true;

        move.onClick.AddListener(() => MoveAction());
        goFirst.onClick.AddListener(() => GoFirstAction());

        ability1.onClick.AddListener(() => Ability(0));
        ability2.onClick.AddListener(() => Ability(1));
        ability3.onClick.AddListener(() => Ability(2));
        ability4.onClick.AddListener(() => Ability(3));
    }
    public void DisableButtons()
    {
        move.enabled = false;
        goFirst.enabled = false;
        ability1.enabled = false;
        ability2.enabled = false;
        ability3.enabled = false;
        ability4.enabled = false;
        
        move.onClick.RemoveListener(() => MoveAction());
        goFirst.onClick.RemoveListener(() => GoFirstAction());

        ability1.onClick.RemoveListener(() => Ability(0));
        ability2.onClick.RemoveListener(() => Ability(1));
        ability3.onClick.RemoveListener(() => Ability(2));
        ability4.onClick.RemoveListener(() => Ability(3));
    }
    public void SetTurnMarker(Transform target)
    {
        turnMarker.position=Camera.main.WorldToScreenPoint(target.position+Vector3.up*1.5f);
    }
    //public void RegisterListner(Action listner)
    //{

    //}
    // Update is called once per frame
    void Update()
    {
        
    }

    
}
