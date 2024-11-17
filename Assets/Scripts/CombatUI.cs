using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatUI : VisualElement
{

    TabbedView tabbedView;
    PlayerTurn currentTurn;
    Button moveButton,endTurnButton;
    AbilityButton[] abilityButtons=new AbilityButton[4];
    ProgressBar healthBar, staminaBar;
    VisualElement turnMarker;
    VisualElement turnList;
    VisualElement itemBar;
    ItemInventory inventory;
    VisualElement statusBar;
    AbilityToolTip toolTip;
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

        this.StretchToParentSize();
        this.pickingMode = PickingMode.Ignore;
        toolTip = new AbilityToolTip();
        this.Add(toolTip);
        tabbedView = this.Q<CombatTabs>("CombatTabs");
        
        moveButton = tabbedView.Q<Button>("Move");
        moveButton.clicked +=Move;
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i] = tabbedView.Q<AbilityButton>("ability" + i);
            int index = i;
            abilityButtons[i].clicked += () => UseAbility(index);

            abilityButtons[i].mouseEnter += (action) => {action(toolTip);};
            abilityButtons[i].mouseExit += () => toolTip.visible = false;
        }
        endTurnButton = this.Q<Button>("EndTurn");
        endTurnButton.clicked += EndTurn;
        staminaBar = this.Q<ProgressBar>("StaminaBar");
        healthBar = this.Q<ProgressBar>("HealthBar");
        turnMarker = this.Q("TurnMarker");
        turnList = this.Q("TurnList");
        itemBar = this.Q("Items");
        statusBar = this.Q("StatusBar");
        
        
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
        //turnList.Add(turn);

    }
    void EndTurn()
    {
        currentTurn.EndTurn();
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
            if (currentTurn.AbilityUsable(i))
            {
                abilityButtons[i].SetEnabled(true);
            }
            else
            {
                abilityButtons[i].SetEnabled(false);
            }

        }
        endTurnButton.SetEnabled(true);
        

    }
    public void DisableButtons()
    {
        moveButton.SetEnabled(false);
        foreach (AbilityButton button in abilityButtons)
        {
            button.SetEnabled(false);
        }
        endTurnButton.SetEnabled(false);
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
            float damage = currentTurn.fish.GetAbility(i).GetDamage(currentTurn);
            //abilityButtons[i].UpdateVisuals(currentTurn.fish.GetAbility(i), damage);
            abilityButtons[i].SetAbility(currentTurn.fish.GetAbility(i),damage);
        }
        //if (actionTokens != null)
        //{
        //    foreach (var token in actionTokens)
        //    {
        //        if (token != null)
        //        {
        //            Destroy(token.gameObject);
        //        }

        //    }
        //    actionTokens.Clear();
        //}

        //for (int i = 0; i < currentTurn.actionsPerTurn; i++)
        //{
        //    actionTokens.Add(Instantiate(actionTokenPrefab, actionsLeftBar).GetComponent<ActionToken>());
        //}

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
                icon.mouseEnter += (action) => action(toolTip);
                icon.mouseOut +=()=>toolTip.visible=false;
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
        var combatItems = inv.GetDictionaryOfItems<CombatItem>();
        foreach (var combatItem in combatItems)
        {
            var itemUI = new CombatItemUI(combatItem.Key, combatItem.Value);
            itemBar.Add(itemUI);
            itemUI.Clicked+=UseItem;

        }
    }
    
    public void UpdateInventory()
    {
        var combatItems = inventory.GetDictionaryOfItems<CombatItem>();
        var  uiItems = itemBar.Children().ToArray();
        foreach (CombatItemUI uiItem in  uiItems)
        {
            if (combatItems.ContainsKey(uiItem.item))
            {
                uiItem.SetAmount(combatItems[uiItem.item]);
            }
            else
            {
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
        status.mouseEnter += (action) => action(toolTip);
        status.mouseOut += () => toolTip.visible = false;
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
