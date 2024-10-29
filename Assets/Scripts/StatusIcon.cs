using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    [SerializeField]
    Image imageObj;
    [SerializeField]
    TextMeshProUGUI turnsLeftText;
    StatusEffect.StatusEffectInstance statusEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetEffect(StatusEffect.StatusEffectInstance statusEffect)
    {
        this.statusEffect = statusEffect;
        if (statusEffect.effect.Icon != null)
        {
            imageObj.sprite = this.statusEffect.effect.Icon;
        }
        UpdateIcon(statusEffect.remainingDuration);
        this.statusEffect.DurationChanged += UpdateIcon;
    }
    public void UpdateIcon(int duration)
    {
        turnsLeftText.text = duration.ToString();
    }
}
