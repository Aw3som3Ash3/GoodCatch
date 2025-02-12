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
    
    VisualElement moreInfoScreen;
    float moreInfoBaseWidth;
    bool moreInfoOpen;
    VisualElement moreInfoButton;
    Clickable moreInfoClickable;

    Label infoScreenHealth, infoScreenStamina, infoScreenAttack,infoScreenMagicAttack,infoScreenDefense,infoScreenMagicDefense,infoScreenAgility;

    readonly GoodCatchInputs inputs= new();

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
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/CombatUI");

        visualTreeAsset.CloneTree(root);
        this.style.flexGrow = 1;
      
        inputs.Combat.Enable();
        this.StretchToParentSize();
        this.pickingMode = PickingMode.Ignore;
        this.Q("MainCombat").pickingMode = PickingMode.Ignore;
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
        inputs.Combat.MoreInfo.performed += OnMoreInfo;

        moreInfoButton = this.Q("IndicatorImageBox");
        moreInfoScreen = this.Q("AlfredInfo");
      
        moreInfoOpen = true;
        moreInfoButton.AddManipulator(moreInfoClickable);
        moreInfoButton.RegisterCallback<ClickEvent>((e) => MoreInfo());
        
        infoScreenAgility = this.Q<Label>("AgilityAmount");
        infoScreenAttack = this.Q<Label>("AtkAmount");
        infoScreenDefense = this.Q<Label>("DefAmount");
        infoScreenMagicAttack = this.Q<Label>("MgAtkAmount");
        infoScreenMagicDefense = this.Q<Label>("MgDefAmount");
        infoScreenHealth = this.Q<Label>("HealthAmount");
        infoScreenStamina = this.Q<Label>("StamAmount");
        
        GameManager.Instance.OnInputChange += OnInputChange;
        PauseMenu.GamePaused += EnableUI;

    }
    void OnMoreInfo(InputAction.CallbackContext context)
    {
        MoreInfo();
    }
    void MoreInfo()
    {
        if (moreInfoOpen)
        {
            moreInfoBaseWidth = moreInfoScreen.layout.width;
            Debug.Log("base width" + moreInfoBaseWidth);
        }
        moreInfoOpen = !moreInfoOpen;
        var width = moreInfoScreen.style.width.value;
        width.value = moreInfoOpen? moreInfoBaseWidth:0;
        moreInfoScreen.style.width = width;
        moreInfoScreen.Children().ToArray()[0].visible = moreInfoOpen;
        moreInfoScreen.Children().ToArray()[1].visible = moreInfoOpen;
    }
    void MoreInfo(bool open)
    {
        moreInfoOpen = open;
        MoreInfo();
    }

    void UpdateInfo(FishMonster fish)
    {
        infoScreenAgility.text =fish.Agility.value.ToString();
        infoScreenAttack.text = fish.Attack.value.ToString();
        infoScreenDefense.text = fish.Fortitude.value.ToString();
        infoScreenMagicAttack.text = fish.Special.value.ToString();
        infoScreenMagicDefense.text = fish.SpecialFort.value.ToString();
        infoScreenHealth.text = fish.Health.ToString("00")+"/"+fish.MaxHealth.ToString("00");
        infoScreenStamina.text = fish.MaxStamina.ToString("00");
        if (fish.Type.Elements.Length >=1)
        {
            this.Q<Label>("Type1Amount").text = fish.Type.Elements[0].name;

        }
        else
        {
            this.Q<Label>("Type1Amount").text ="";
        }
        if (fish.Type.Elements.Length >= 2)
        {
            this.Q<Label>("Type2Amount").text = fish.Type.Elements[1].name;
        }
        else
        {
            this.Q<Label>("Type2Amount").text = "";
        }
           
    }
    private void OnInputChange(InputMethod method)
    {
        if (method == InputMethod.controller)
        {
            FocusOn();
        }
        
    }

    void FocusOn(int index=0)
    {

        if (combatDraftUI.enabledSelf)
        {
            combatDraftUI.Q<Button>("slot" + (1)).Focus();
        }
        else
        {
            tabbedView.FocusOn(index);
            
        }
    }
    
    private void OnRun()
    {
        throw new NotImplementedException();
    }

    public void Draft(IList<FishMonster> playerFishes, Action<int,Action<bool>> callback)
    {
        //ResetDisplayAbilities();
        tabbedView.SetEnabled(false);
        combatDraftUI.SetEnabled(true);
        combatDraftUI.visible = true;
        endTurnButton.SetEnabled(false);
        bool isSelected = false;
        for (int i = 0;i < playerFishes.Count;i++)
        {
            
            int index = i;
            var slot = combatDraftUI.Q<Button>("slot" + (i + 1));
            var value = slot.style.backgroundImage.value;
            value.sprite=playerFishes[i].Icon;
            slot.style.backgroundImage=value;
            if (playerFishes[i].Health <= 0)
            {
                slot.style.unityBackgroundImageTintColor = Color.gray;
                slot.SetEnabled(false);
                continue;
            }

            slot.RegisterCallback<PointerOverEvent>((e) =>
            {
                if (!isSelected)
                {
                    PreivewFish(playerFishes[index]);
                }
               
            });
            slot.RegisterCallback<FocusInEvent>((e) =>
            {
                if (!isSelected)
                {
                    PreivewFish(playerFishes[index]);
                }
            });


            slot.clicked += () =>
            {
                isSelected = true;
                slot.AddToClassList("DraftSelected");
                PreivewFish(playerFishes[index]);
                callback(index, (Completed) =>
                {
                    isSelected = false;
                    slot.RemoveFromClassList("DraftSelected");
                    if (Completed)
                    {
                        slot.style.unityBackgroundImageTintColor = Color.gray;
                        //var nextSelectedSlot = combatDraftUI.Q<Button>("slot" + (index + 1));
                        //nextSelectedSlot.Focus();
                        slot.SetEnabled(false);
                    }

                    if (GameManager.Instance.inputMethod == InputMethod.controller)
                    {
                        int k = 0;
                        Button nextSelectedSlot;
                        if (Completed)
                        {
                            do
                            {

                                nextSelectedSlot = combatDraftUI.Q<Button>("slot" + (k + 1));
                                k++;

                            } while (!nextSelectedSlot.enabledSelf);
                        }
                        else
                        {
                            nextSelectedSlot = combatDraftUI.Q<Button>("slot" + (index + 1));
                          
                        }

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
        inputs.Combat.ChangeTab.performed += ChangeTab;
    }
    
    void ChangeTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            toolTip.visible = false;
            tabbedView.ChangeTab((int)context.ReadValue<float>());
            Debug.Log("tab change: " + (int)context.ReadValue<float>());
            

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
        

    }
    void Move()
    {
        currentTurn.Move(() => { 
            if (GameManager.Instance.inputMethod == InputMethod.controller)
            {
                //FocusOn(0);
            }
        });
    }
    public void NewTurn(CombatManager.Turn turn, bool isPlayer)
    {
        if (isPlayer && turn is PlayerTurn)
        {
            UpdateVisuals(turn as PlayerTurn);
            EnableButtons();
           
            tabbedView.ChangeTab(-3);
            if (GameManager.Instance.inputMethod == InputMethod.controller)
            {
                abilityButtons[0].Focus();
            }
           
        }
        else
        {
            DisableButtons();
        }
    }
    void UseAbility(int index)
    {
        abilityButtons[index].AddToClassList("AbilitySelected");
        currentTurn.UseAbility(index, () => 
        {
            if (GameManager.Instance.inputMethod==InputMethod.controller)
            {
                abilityButtons[index].RemoveFromClassList("AbilitySelected");
                FocusOn(index+1);
            }
        });
       // AbilityAction?.Invoke(index);
    }
    void PreivewFish(FishMonster fish)
    {
        fishName.text = fish.Name;
        level.text = fish.Level.ToString();
        var value = fishIcon.style.backgroundImage.value;
        value.sprite = fish.Icon;
        fishIcon.style.backgroundImage = value;
        tabbedView.SetEnabled(true);
        UpdateHealthDisplayer(fish);
        UpdateInfo(fish);
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            float damage = fish.GetAbility(i).GetDamage(fish);
            abilityButtons[i].SetAbility(fish.GetAbility(i), damage, fish.Accuracy.value);
            abilityButtons[i].SetEnabled(true);
            abilityButtons[i].SetPreview(true);
            //if (currentTurn.AbilityUsable(i))
            //{
            //    abilityButtons[i].SetUsability(true);
            //}
            //else
            //{
            //    abilityButtons[i].SetUsability(false);
            //}

        }
    }
    public void ResetFishPreview()
    {
        fishName.text = "";
        level.text ="";
        var value = fishIcon.style.backgroundImage.value;
        value.sprite = null;
        fishIcon.style.backgroundImage = value;
        ResetHealthDisplay();
        //tabbedView.SetEnabled(true);
        for (int i = 0; i < abilityButtons.Length; i++)
        {

            abilityButtons[i].SetEnabled(false);
            abilityButtons[i].SetPreview(false);
            //if (currentTurn.AbilityUsable(i))
            //{
            //    abilityButtons[i].SetUsability(true);
            //}
            //else
            //{
            //    abilityButtons[i].SetUsability(false);
            //}

        }
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
            this.currentTurn.fish.ValueChanged -= UpdateHealthDisplay;
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
            abilityButtons[i].SetAbility(currentTurn.fish.GetAbility(i),damage,currentTurn.accuracy);
        }
        fishName.text = currentTurn.fish.Name;
        level.text = currentTurn.fish.Level.ToString();
        var value = fishIcon.style.backgroundImage.value;
        value.sprite= currentTurn.fish.Icon;
        fishIcon.style.backgroundImage= value;
        ResetEffects();
        SetEffects();
        currentTurn.NewEffect += AddEffect;
        currentTurn.fish.ValueChanged += UpdateHealthDisplay;
        UpdateInfo(currentTurn.fish);
        UpdateHealthDisplay();
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
    private void ResetHealthDisplay()
    {
        healthBar.value = 0;
        staminaBar.value = 0;
    }
    private void UpdateHealthDisplay()
    {
        healthBar.value = currentTurn.Health/currentTurn.MaxHealth;
        healthBar.Q<Label>("HealthValue").text = currentTurn.Health.ToString("00") + "/" + currentTurn.MaxHealth.ToString("00");
        staminaBar.value = currentTurn.Stamina/currentTurn.MaxStamina;
        //staminaBar.Q<Label>("StaminaValue").text = currentTurn.Stamina + "/" + currentTurn.MaxStamina;

    }
    private void UpdateHealthDisplayer(FishMonster fish)
    {
        healthBar.value = fish.Health / fish.MaxHealth;
        healthBar.Q<Label>("HealthValue").text = fish.Health.ToString("00") + "/" + fish.MaxHealth.ToString("00");
        staminaBar.value = fish.Stamina / fish.MaxStamina;
        //staminaBar.Q<Label>("StaminaValue").text = fish.Stamina + "/" + fish.MaxStamina;

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
        currentTurn.UseItem(item,()=>FocusOn());
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
        this.Q("ConditionArea").Add(fishUI);
        return fishUI;
    }

    void EnableUI(bool b)
    {
        this.SetEnabled(!b);
    }

    ~CombatUI() 
    {
        PauseMenu.GamePaused -= EnableUI;
    }
}
