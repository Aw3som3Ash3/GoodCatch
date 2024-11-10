using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class FishUI : VisualElement
{
    CombatManager.Turn turn;
    [SerializeField]
    ProgressBar healthBar, staminaBar;
    [SerializeField]
    VisualElement statusBar;



    Transform target;
    Dictionary<StatusEffect.StatusEffectInstance, StatusIcon> statusIcon = new Dictionary<StatusEffect.StatusEffectInstance, StatusIcon>();

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
        var origin = this.style.transformOrigin.value;
        //origin.x = 500f;
        this.style.transformOrigin = origin;
        healthBar = this.Q<ProgressBar>("HealthBar");
        staminaBar = this.Q<ProgressBar>("StaminaBar");
        SetFish(turn, target);
    }
    // Update is called once per frame
    public void UpdatePosition()
    {
        if (target != null)
        {
            
            Vector2 pos = Camera.main.WorldToViewportPoint(target.transform.position+Vector3.down);
            Debug.Log(pos);
            this.transform.position = new Vector2(pos.x * this.parent.worldBound.width, (1-pos.y) * this.parent.worldBound.height);
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

        //Destroy(statusIcon[instance].gameObject);
        statusIcon.Remove(instance);

    }

    private void NewEffect(StatusEffect.StatusEffectInstance instance)
    {
        //var icon = Instantiate(statusIconPrefab, statusBar).GetComponent<StatusIcon>();
        Debug.Log(instance);
        //Debug.Log(icon);
        //icon.SetEffect(instance);
        //statusIcon.Add(instance, icon);

    }

    void UpdateUI()
    {
        healthBar.value = turn.Health / turn.MaxHealth;
        staminaBar.value = turn.Stamina / turn.MaxStamina;
    }

    private void OnDestroy()
    {
        this.turn.fish.ValueChanged -= UpdateUI;
    }
}
