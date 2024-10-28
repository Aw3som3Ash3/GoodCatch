using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    Button move,goFirst,endTurn;
    [SerializeField]
    AbilityButton[] abilityButtons;
    //public Action GoFirstAction,EndTurn;
    Action<int> DepthSelection;
    public Action<int, int> AbilityAction;
    public Action<FishMonster, int> MoveAction;
    bool isActive;
    [SerializeField]
    RectTransform turnMarker;
    [SerializeField]
    GameObject depthSelectorMark;
    [SerializeField]
    List<DepthSelectors> depthSelectors;
    Transform turnTarget;

    //FishMonster currentFish;
    PlayerTurn currentTurn;

    [SerializeField]
    RectTransform actionsLeftBar;
    [SerializeField]
    GameObject actionTokenPrefab;
    List<ActionToken> actionTokens=new List<ActionToken>();

    EventSystem eventSystem;
    int selected;
    private void Awake()
    {
        CombatManager.Turn.NewTurn +=(turn,b)=> {if (b && turn is PlayerTurn) { UpdateVisuals(turn as PlayerTurn); EnableButtons();  } else { DisableButtons(); } };
    }
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        InputManager.Input.UI.Enable();
        InputManager.Input.UI.RightClick.performed += (x)=> StopTargeting();
        move.onClick.AddListener(() =>Move());
        //goFirst.onClick.AddListener(() => GoFirstAction());
       

        for(int i = 0; i < abilityButtons.Length; i++)
        {

            abilityButtons[i].SetIndex(i);
            abilityButtons[i].OnHover += OnHover;
            abilityButtons[i].OnHoverExit += OnHoverExit;
            abilityButtons[i].Subscribe(Attack);
        }
        endTurn.onClick.AddListener(() => currentTurn.EndTurn());
        for(int i = 0; i < depthSelectors.Count; i++)
        {
            depthSelectors[i].SetIndex(i);
            depthSelectors[i].Selected = (i)=>{ DepthSelection?.Invoke(i); StopTargeting(); };
            depthSelectors[i].Navigate +=OnNaviagte;
            
        }

    }

    void OnNaviagte(float i)
    {
        float value = i;

        if (value > 0)
        {
            selected++;

        }
        else if (value < 0)
        {
            selected--;

        }
        selected = Mathf.Clamp(selected, 0, depthSelectors.Count - 1);
        print(selected);

        eventSystem.SetSelectedGameObject(depthSelectors[selected].gameObject);

    }
    void OnHover(int index)
    {
        print("hovered: "+index);
        print(currentTurn);
        if (!currentTurn.ActionLeft|| !currentTurn.AbilityUsable(index))
        {
            return;
        }
        foreach (DepthSelectors selector in depthSelectors)
        {
            if (currentTurn.DepthTargetable(index,selector.CurrentDepth))
            {
                selector.PreviewSelection(true);

            }
            else
            {
                selector.PreviewSelection(false);
            }

        }
    }

    void Move()
    {
        if (!currentTurn.ActionLeft)
        {
            return;
        }
        //DepthSelection = (i) => MoveAction?.Invoke(this.currentTurn.fish, i);
        DepthSelection = (i)=> { currentTurn.Move(i); EnableButtons(); UpdateActionsLeft(); };
        StartTargeting();
    }
    void Attack(int abilityIndex)
    {
        if (!currentTurn.ActionLeft||!currentTurn.AbilityUsable(abilityIndex))
        {
            return;
        }
        if (currentTurn.fish.GetAbility(abilityIndex).Targeting == Ability.TargetingType.all)
        {
            currentTurn.UseAbility(abilityIndex, -1);
            EnableButtons();
            UpdateActionsLeft();
            
        }
        else
        {
            DepthSelection = (i) => { currentTurn.UseAbility(abilityIndex, i); EnableButtons(); UpdateActionsLeft(); };
            StartTargeting(abilityIndex, currentTurn.fish.GetAbility(abilityIndex).TargetableDepths);
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
        for(int i=0;i<abilityButtons.Length;i++)
        {
            if (currentTurn.AbilityUsable(i))
            {
                abilityButtons[i].enabled = true;
            }
            else
            {
                abilityButtons[i].enabled = false;
            }
           
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
 
    // Update is called once per frame
    void Update()
    {
        if (turnTarget==null)
        {
            return;
        }
        turnMarker.position = Camera.main.WorldToScreenPoint(turnTarget.position + Vector3.up * 1.5f);

    }
    void StartTargeting(int index,Depth targetableDepths)
    {
        foreach (DepthSelectors selector in depthSelectors)
        {
            if (currentTurn.DepthTargetable(index, selector.CurrentDepth))
            {
                eventSystem.SetSelectedGameObject(selector.gameObject);
                selected=depthSelectors.IndexOf(selector);
                selector.SetSelection(true);
            }
            else
            {
                selector.SetSelection(false);
            }
            
        }
        
        
    }
    void StartTargeting()
    {
        eventSystem.SetSelectedGameObject(depthSelectors[0].gameObject);
        foreach (DepthSelectors selector in depthSelectors)
        {
           
            selector.SetSelection(true);
        }
       
    }
    void StopTargeting()
    {
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.SetSelection(false);
            selector.PreviewSelection(false);
        }
        DepthSelection = null;
        //isActive = true;
    }
    public void UpdateVisuals(PlayerTurn currentTurn)
    {
        print(currentTurn);
        this.currentTurn = currentTurn as PlayerTurn;
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
            abilityButtons[i].UpdateVisuals(currentTurn.fish.GetAbility(i)?.name, currentTurn.fish.GetAbility(i)?.Icon);
            
        }


    }
    public void UpdateActionsLeft()
    {
        for(int i = currentTurn.actionsLeft; i < actionTokens.Count; i++)
        {
            actionTokens[i].Use();
        }
    }

}
