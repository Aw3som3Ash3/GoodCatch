using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    Button move,goFirst,endTurn;
    [SerializeField]
    AbilityButton[] abilityButtons;
    public Action MoveAction, GoFirstAction,EndTurn;
    public Action<int> Ability,DepthSelection;
    bool isActive;
    [SerializeField]
    RectTransform turnMarker;
    [SerializeField]
    GameObject depthSelectorMark;
    [SerializeField]
    DepthSelectors[] depthSelectors;
    Transform turnTarget;
    DepthSelectors prevDepth;

    FishMonster currentFish;
    CombatManager.Turn currentTurn;

    [SerializeField]
    RectTransform actionsLeftBar;
    [SerializeField]
    GameObject actionTokenPrefab;
    List<ActionToken> actionTokens=new List<ActionToken>();
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Input.UI.Enable();
        move.onClick.AddListener(() => MoveAction());
        goFirst.onClick.AddListener(() => GoFirstAction());

        for(int i = 0; i < abilityButtons.Length; i++)
        {

            abilityButtons[i].SetIndex(i);
            abilityButtons[i].OnHover += OnHover;
            abilityButtons[i].OnHoverExit += OnHoverExit;
            abilityButtons[i].Subscribe(Ability);
        }
        endTurn.onClick.AddListener(() => EndTurn());
        for(int i = 0; i < depthSelectors.Length; i++)
        {
            int index = i;
            depthSelectors[i].Selected +=()=> DepthSelection(index);
        }

    }
    void OnHover(int index)
    {
        print("hovered: "+index);
        if (!currentTurn.ActionLeft)
        {
            return;
        }
        foreach (DepthSelectors selector in depthSelectors)
        {
            if (currentFish.GetAbility(index).TargetableDepths.HasFlag(selector.CurrentDepth))
            {
                selector.PreviewSelection(true);

            }
            else
            {
                selector.PreviewSelection(false);
            }

        }
    }
    void OnHoverExit(int index)
    {
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.PreviewSelection(false);
        }
    }
    public void EnableButtons()
    {

        move.enabled = true;
        goFirst.enabled = true;
        foreach(AbilityButton button in abilityButtons)
        {
            button.enabled = true;
        }
        endTurn.enabled = true;
        isActive = true;

    }
    public void DisableButtons()
    {
        move.enabled = false;
        goFirst.enabled = false;
        foreach (AbilityButton button in abilityButtons)
        {
            button.enabled = false;
        }
        endTurn.enabled = false;
        isActive = false;
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
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(InputManager.Input.UI.Point.ReadValue<Vector2>()), out hit))
        {
            print("hit");
            if(prevDepth!= hit.collider.GetComponent<DepthSelectors>())
            {
                if (prevDepth != null)
                {
                    prevDepth.OnHover(false);
                }
                prevDepth = hit.collider.GetComponent<DepthSelectors>();
                prevDepth.OnHover(true);
            }
            
        }
        else if(prevDepth!=null)
        {
            prevDepth.OnHover(false);
            prevDepth = null;
        }
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
    public void StartTargeting(Depth targetableDepths)
    {
        //isTargeting = true;
        InputManager.Input.UI.Click.performed += OnClick;
        foreach (DepthSelectors selector in depthSelectors)
        {
            if (targetableDepths.HasFlag(selector.CurrentDepth))
            {
                selector.SetSelection(true);
            }
            else
            {
                selector.SetSelection(false);
            }
            
        }
        
    }
    public void StartTargeting()
    {
        //isTargeting = true;
        InputManager.Input.UI.Click.performed += OnClick;
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.SetSelection(true);
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

        //isActive = true;
    }
    public void UpdateVisuals(CombatManager.Turn currentTurn)
    {
        this.currentTurn = currentTurn;
        currentFish = currentTurn.fish;
        if (actionTokens != null)
        {
            foreach (var token in actionTokens)
            {
                Destroy(token.gameObject);
            }
            actionTokens.Clear();
        }
       
        for(int i = 0; i < currentTurn.actionsPerTurn;i++)
        {
            actionTokens.Add(Instantiate(actionTokenPrefab, actionsLeftBar).GetComponent<ActionToken>());
        }
        for (int i = 0;i<abilityButtons.Length;i++)
        {
            abilityButtons[i].UpdateVisuals(currentFish.GetAbility(i)?.name, currentFish.GetAbility(i)?.Icon);
        }


    }
    public void UpdateActionsLeft(int amountLeft)
    {
        for(int i = amountLeft; i < actionTokens.Count; i++)
        {
            actionTokens[i].Use();
        }
    }

}
