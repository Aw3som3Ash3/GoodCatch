using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    Button move,goFirst,ability1,ability2,ability3,ability4,endTurn;

    public Action MoveAction, GoFirstAction,EndTurn;
    public Action<int> Ability,DepthSelection;
    bool isActive;
    [SerializeField]
    RectTransform turnMarker;
    [SerializeField]
    DepthSelectors[] depthSelectors;
    Transform turnTarget;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Input.UI.Enable();
        move.onClick.AddListener(() => MoveAction());
        goFirst.onClick.AddListener(() => GoFirstAction());

        ability1.onClick.AddListener(() => Ability(0));
        ability2.onClick.AddListener(() => Ability(1));
        ability3.onClick.AddListener(() => Ability(2));
        ability4.onClick.AddListener(() => Ability(3));
        endTurn.onClick.AddListener(() => EndTurn());
        for(int i = 0; i < depthSelectors.Length; i++)
        {
            int index = i;
            depthSelectors[i].Selected +=()=> DepthSelection(index);
        }

    }

    public void EnableButtons()
    {

        move.enabled = true;
        goFirst.enabled = true;
        ability1.enabled = true;
        ability2.enabled = true;
        ability3.enabled = true;
        ability4.enabled = true; 
        endTurn.enabled = true;
        
        
    }
    public void DisableButtons()
    {
        move.enabled = false;
        goFirst.enabled = false;
        ability1.enabled = false;
        ability2.enabled = false;
        ability3.enabled = false;
        ability4.enabled = false;
        endTurn.enabled = false;
       
    }
    public void SetTurnMarker(Transform target)
    {
        turnTarget=target;
       
    }
    //public void RegisterListner(Action listner)
    //{

    //}
    // Update is called once per frame
    void Update()
    {
        if (turnTarget==null)
        {
            return;
        }
        turnMarker.position = Camera.main.WorldToScreenPoint(turnTarget.position + Vector3.up * 1.5f);
    }
    public void UpdateUI(FishMonster fish)
    {

    }
    void OnClick(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(InputManager.Input.UI.Point.ReadValue<Vector2>()),out hit))
        {
            print("hit");
            hit.collider.GetComponent<DepthSelectors>()?.SelectDepth();

        }
    }
    public void StartTargeting()
    {
        //isTargeting = true;
        InputManager.Input.UI.Click.performed += OnClick;
        foreach (DepthSelectors selectors in depthSelectors)
        {
            selectors.SetSelection(true);
        }

    }
    public void StopTargeting()
    {
        //isTargeting = false;
        InputManager.Input.UI.Click.performed -= OnClick;
        foreach (DepthSelectors selectors in depthSelectors)
        {
            selectors.SetSelection(false);
        }


    }

}
