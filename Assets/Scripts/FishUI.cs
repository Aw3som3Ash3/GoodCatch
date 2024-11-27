using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class FishUI : VisualElement
{
    CombatManager.Turn turn;
    ProgressBar healthBar, staminaBar;
   
    VisualElement statusBar;

    Transform target;
    Dictionary<StatusEffect.StatusEffectInstance, StatusIcon> statusIcon = new Dictionary<StatusEffect.StatusEffectInstance, StatusIcon>();

    public Action<Action<ToolTipBox>> onHoverStatus;
    public Action onHoverExit;
    public new class UxmlFactory : UxmlFactory<FishUI, FishUI.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {

    }
    public FishUI()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/FishConditionBar.uxml");
        
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        
    }

    public FishUI(CombatManager.Turn turn, Transform target)
    {
        
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/UI/FishConditionBar.uxml");
        visualTreeAsset.CloneTree(root);
        this.style.position = Position.Absolute;
        this.transform.position = new Vector3(-55, 0);
        
        healthBar = this.Q<ProgressBar>("HealthBar");
        staminaBar = this.Q<ProgressBar>("StaminaBar");
        SetFish(turn, target);
        statusBar = this.Q("StatusBar");
    }
    // Update is called once per frame
    public void UpdatePosition()
    {
        if (target != null&&parent!=null)
        {
            
            Vector2 pos = Camera.main.WorldToViewportPoint(target.transform.position+Vector3.down);
            //Debug.Log(pos);
            
            this.style.left = pos.x * this.parent.worldBound.width;
            this.style.top = (1 - pos.y) * this.parent.worldBound.height;
        }
    }

    void SetFish(CombatManager.Turn turn, Transform target)
    {

        //this.fish.ValueChanged -= UpdateUI;
        this.turn = turn;
        this.turn.fish.ValueChanged += UpdateUI;
        this.target = target;
        UpdateUI();
        this.turn.NewEffect += NewEffect;
        this.turn.EffectRemoved += EffectRemoved;
        
        
    }

    private void EffectRemoved(StatusEffect.StatusEffectInstance instance)
    {
        var status = statusIcon[instance];
        statusBar.Remove(status);
        status.MouseEnter -= onHoverStatus;
        status.MouseExit -= onHoverExit;
        statusIcon.Remove(instance);

    }

    private void NewEffect(StatusEffect.StatusEffectInstance instance)
    {
        var status = new StatusIcon(instance);
        status.MouseEnter += onHoverStatus;
        status.MouseExit += onHoverExit;
        statusIcon[instance]=status;
        statusBar.Add(status);
        Debug.Log(instance);


    }

    void UpdateUI()
    {
        healthBar.value = turn.Health / turn.MaxHealth;
        staminaBar.value = turn.Stamina / turn.MaxStamina;
    }

    ~FishUI()
    {
        this.turn.fish.ValueChanged -= UpdateUI;
    }
}
