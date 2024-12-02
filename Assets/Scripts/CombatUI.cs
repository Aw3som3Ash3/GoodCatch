using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CombatUI : VisualElement
{

    CombatTabs tabbedView;
    PlayerTurn currentTurn;
    Button moveButton,endTurnButton,runButton;
    AbilityButton[] abilityButtons=new AbilityButton[4];
    ProgressBar healthBar, staminaBar;
    VisualElement turnMarker;
    VisualElement turnList;
    VisualElement itemBar;
    ItemInventory inventory;
    VisualElement combatDraftUI;
    VisualElement statusBar;
    Label fishName, level;
    VisualElement fishIcon;
    ToolTipBox toolTip;
    Dictionary<CombatManager.Turn, TurnListIcon> turnIcon=new Dictionary<CombatManager.Turn, TurnListIcon>();
    //public Action MoveAction,EndTurnAction;
    //public Action<int> AbilityAction;
    public new class UxmlFactory : UxmlFactory<CombatUI, CombatUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public CombatUI() 
    {
        Initial();
    }
    public void Initial()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/CombatUI.uxml");
        visualTreeAsset.CloneTree(root);
        this.style.flexGrow = 1;
      
        //InputManager.Input.UI.ChangeTab.Enable();
        this.StretchToParentSize();
        this.pickingMode = PickingMode.Ignore;
        toolTip = new ToolTipBox();
        this.Add(toolTip);
        tabbedView = this.Q<CombatTabs>("CombatTabs");
        runButton = this.Q<Button>("RunButton");
        runButton.clicked += OnRun;
        moveButton = tabbedView.Q<Button>("Move");
        moveButton.clicked +=Move;
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i] = tabbedView.Q<AbilityButton>("ability" + i);
            int index = i;
            abilityButtons[i].clicked += () => UseAbility(index);

            abilityButtons[i].MouseEnter += (action) => {action(toolTip);};
            
            abilityButtons[i].MouseExit += () => toolTip.visible = false;
        }
        endTurnButton = this.Q<Button>("EndTurn");
        endTurnButton.clicked += EndTurn;
        staminaBar = this.Q<ProgressBar>("StaminaBar");
        healthBar = this.Q<ProgressBar>("HealthBar");
        turnMarker = this.Q("TurnMarker");
        turnMarker.visible = false;
        turnList = this.Q("TurnList");
        itemBar = this.Q("Items");
        statusBar = this.Q("StatusBar");
        combatDraftUI = this.Q("CombatDraftUI");
        combatDraftUI.SetEnabled(false);
        combatDraftUI.visible = false;
        fishName = this.Q<Label>("Name");
        level = this.Q<Label>("Level");
        fishIcon = this.Q("ProfilePic");


    }

    private void OnRun()
    {
        throw new NotImplementedException();
    }

    public void Draft(IList<FishMonster> playerFishes, Action<int,Action> callback)
    {

        tabbedView.SetEnabled(false);
        combatDraftUI.SetEnabled(true);
        combatDraftUI.visible = true;
        endTurnButton.SetEnabled(false);
        for (int i = 0;i < playerFishes.Count;i++)
        {
            
            int index = i;
            var slot = combatDraftUI.Q<Button>("slot" + (i + 1));
            slot.style.backgroundImage=playerFishes[i].Icon;
            slot.clicked += () =>
            {
                slot.AddToClassList("DraftSelected");
                callback(index, () =>
                {
                    slot.SetEnabled(false);
                    slot.style.unityBackgroundImageTintColor = Color.gray;
                    slot.RemoveFromClassList("DraftSelected");
                    if (GameManager.Instance.inputMethod == InputMethod.controller)
                    {
                        int k = 0;
                        Button nextSelectedSlot;
                        do
                        {
                            nextSelectedSlot = combatDraftUI.Q<Button>("slot" + (k + 1));
                            k++;

                        } while (!nextSelectedSlot.enabledSelf);

                        nextSelectedSlot.Focus();
                    }
                   
                });

            };
            if (i == 0&& GameManager.Instance.inputMethod == InputMethod.controller)
            {
                slot.Focus();
            }

        }
    }
    public void StopDraft()
    {
        combatDraftUI.SetEnabled(false); 
        combatDraftUI.visible = false;
        tabbedView.SetEnabled(true);
        endTurnButton.SetEnabled(true);
        turnMarker.visible = true;
    }
    
    void ChangeTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tabbedView.ChangeTab((int)context.ReadValue<float>());
            Debug.Log("tab change: " + (int)context.ReadValue<float>());
            toolTip.visible = false;

        }
      
    }
    
    public void SetTurnUI(List<CombatManager.Turn> turns)
    {
        turnList.Clear();
        for (int i = 0; i < turns.Count; i++)
        {
            var icon = new TurnListIcon(turns[i].fish.Icon, turns[i].team);
            turnList.Add(icon);
            turnIcon[turns[i]]=icon;
        }
        
    }
    public void RemoveTurn(CombatManager.Turn turn)
    {
        
        turnList.Remove(turnIcon[turn]);
        turnIcon.Remove(turn);
    }
    public void NextTurn()
    {
        var turn = turnList.ElementAt(0);
        turnList.Remove(turn);
        turnList.Add(turn);
        

    }
    void EndTurn()
    {
        currentTurn.EndTurn();
        InputManager.Input.Combat.ChangeTab.performed -= ChangeTab;

    }
    void Move()
    {
        currentTurn.Move();
    }
    public void NewTurn(CombatManager.Turn turn, bool isPlayer)
    {
        if (isPlayer && turn is PlayerTurn)
        {
            UpdateVisuals(turn as PlayerTurn);
            EnableButtons();
            InputManager.Input.Combat.ChangeTab.performed += ChangeTab;
            tabbedView.ChangeTab(-3);
            if (GameManager.Instance.inputMethod == InputMethod.controller)
            {
                 moveButton.Focus();
            }
           
        }
        else
        {
            DisableButtons();
        }
    }
    void UseAbility(int index)
    {
        currentTurn.UseAbility(index);
       // AbilityAction?.Invoke(index);
    }
    public void EnableButtons()
    {

        moveButton.SetEnabled(currentTurn.ActionLeft);
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i].SetEnabled(true);
            if (currentTurn.AbilityUsable(i))
            {
                abilityButtons[i].SetUsability(true);
            }
            else
            {
                abilityButtons[i].SetUsability(false);
            }

        }
        foreach (var item in itemBar.Children())
        {
            item.SetEnabled(currentTurn.ActionLeft);
        }
        endTurnButton.SetEnabled(true);
        if (currentTurn.ActionLeft)
        {
        }
        else
        {
            endTurnButton.Focus();
        }
        

    }
    public void DisableButtons()
    {
        moveButton.SetEnabled(false);
        foreach (AbilityButton button in abilityButtons)
        {
            button.SetEnabled(false);
        }
        endTurnButton.SetEnabled(false);
        foreach (var item in itemBar.Children())
        {
            item.SetEnabled(false);
        }
    }

    public void UpdateVisuals(PlayerTurn currentTurn)
    {
        if (this.currentTurn != null)
        {
            this.currentTurn.NewEffect -= AddEffect;
            this.currentTurn.fish.ValueChanged -= UpdateHealth;
        }

        Debug.Log(currentTurn);
        this.currentTurn = currentTurn as PlayerTurn;


        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (currentTurn.fish.GetAbility(i) == null)
            {
                continue;
            }
            float damage = currentTurn.fish.GetAbility(i).GetDamage(currentTurn);
            abilityButtons[i].SetAbility(currentTurn.fish.GetAbility(i),damage);
        }
        fishName.text = currentTurn.fish.Name;
        level.text = currentTurn.fish.Level.ToString();
        fishIcon.style.backgroundImage = currentTurn.fish.Icon;
        ResetEffects();
        SetEffects();
        currentTurn.NewEffect += AddEffect;
        currentTurn.fish.ValueChanged += UpdateHealth;
        UpdateHealth();
    }

    private void SetEffects()
    {

        foreach (var effect in currentTurn.effects)
        {
            if (effect.remainingDuration > 0)
            {
                var icon = new StatusIcon(effect);
                icon.MouseEnter += (action) => action(toolTip);
                icon.MouseExit +=()=>toolTip.visible=false;
                statusBar.Add(icon);

                Debug.Log("setting effcts");

            }
        }
    }
    private void ResetEffects()
    {
       
        var list = statusBar.Children().ToArray();
        foreach (var child in list )
        {
            statusBar.Remove(child);
        }
    }

    private void UpdateHealth()
    {
        healthBar.value = currentTurn.Health/currentTurn.MaxHealth;
        staminaBar.value = currentTurn.Stamina/currentTurn.MaxStamina;
        
    }
    public void SetInventory(ItemInventory inv)
    {
        this.inventory = inv;
        var combatItems = inv.GetListOfItems<CombatItem>();
        foreach (var combatItem in combatItems)
        {
            var itemUI = new CombatItemUI(combatItem.Item, combatItem.amount);
            itemUI.MouseEnter += (action) => action(toolTip);
            itemUI.MouseExit+=() => toolTip.visible = false;
            itemBar.Add(itemUI);
            itemUI.Clicked+=UseItem;

        }
    }
    
    public void UpdateInventory()
    {
        var combatItems = inventory.GetListOfItems<CombatItem>();
        var  uiItems = itemBar.Children().ToArray();
        foreach (CombatItemUI uiItem in  uiItems)
        {
            if (inventory.Contains(uiItem.item))
            {
                uiItem.SetAmount(inventory.GetAmount(uiItem.item));
            }
            else
            {
                toolTip.visible = false;
                itemBar.Remove(uiItem);
                
            }
           
        }
        //foreach (var combatItem in combatItems)
        //{
        //    var itemUI = new CombatItemUI(combatItem.Key, combatItem.Value);
        //    itemBar.Add(itemUI);
        //    itemUI.Clicked += UseItem;

        //}
    }
    void UseItem(Item item)
    {
        currentTurn.UseItem(item);
    }
    private void AddEffect(StatusEffect.StatusEffectInstance instance)
    {
        var status = new StatusIcon(instance);
        status.MouseEnter += (action) => action(toolTip);
        status.MouseExit += () => toolTip.visible = false;
        statusBar.Add(status);
    }
    public void SetTurnMarker(Transform target)
    {
        Vector2 pos= Camera.main.WorldToViewportPoint( target.transform.position + Vector3.up * 1.5f);
        turnMarker.transform.position = new Vector2(pos.x * worldBound.width,(1 - pos.y)* worldBound.height);
        Debug.Log("turn marker position: "+ pos);
    }
    public FishUI AddFishUI(CombatManager.Turn turn, Transform target)
    {
        var fishUI = new FishUI(turn, target);
        
        fishUI.onHoverStatus += (action) => action(toolTip);
        fishUI.onHoverExit += () => toolTip.visible = false;
        Add(fishUI);
        return fishUI;
    }
}
