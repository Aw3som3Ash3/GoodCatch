using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishUI : MonoBehaviour
{
    CombatManager.Turn turn;
    [SerializeField]
    Image healthBar, staminaBar;
    [SerializeField]
    Transform statusBar;

    [SerializeField]
    GameObject statusIconPrefab;

    Transform target;
    Dictionary<StatusEffect.StatusEffectInstance, StatusIcon> statusIcon = new Dictionary<StatusEffect.StatusEffectInstance, StatusIcon>();
    // Start is called before the first frame update
    void Start()
    {
        //this.transform.rotation=Camera.main.transform.rotation;
         
    }
    private void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform != null)
        {
            this.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(target.position - Vector3.up * 1.25f);
        }

    }
    public void SetFish(CombatManager.Turn turn, Transform target)
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

        Destroy(statusIcon[instance].gameObject);
        statusIcon.Remove(instance);

    }

    private void NewEffect(StatusEffect.StatusEffectInstance instance)
    {
        var icon = Instantiate(statusIconPrefab, statusBar).GetComponent<StatusIcon>();
        Debug.Log(instance);
        Debug.Log(icon);
        icon.SetEffect(instance);
        statusIcon.Add(instance, icon);

    }

    void UpdateUI()
    {
        healthBar.fillAmount = turn.Health / turn.MaxHealth;
        staminaBar.fillAmount = turn.Stamina / turn.MaxStamina;
    }

    private void OnDestroy()
    {
        this.turn.fish.ValueChanged -= UpdateUI;
    }
}
