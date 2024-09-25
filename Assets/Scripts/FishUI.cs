using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishUI : MonoBehaviour
{
    FishMonster fish;

    [SerializeField]
    Image healthBar, staminaBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        fish.ValueChanged += UpdateUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI()
    {
        healthBar.fillAmount = fish.health / fish.maxHealth;
        staminaBar.fillAmount = fish.stamina / fish.maxStamina;
    }


}
