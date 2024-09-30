using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishUI : MonoBehaviour
{
    FishMonster fish;

    [SerializeField]
    Image healthBar, staminaBar;
    Transform target;
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
            this.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(target.position-Vector3.up*1.25f);
        }
        
    }
    public void SetFish(FishMonster fish,Transform target)
    {
        //this.fish.ValueChanged -= UpdateUI;
        this.fish = fish;
        this.fish.ValueChanged += UpdateUI;
        this.target = target;
    }
    void UpdateUI()
    {
        healthBar.fillAmount = fish.health / fish.maxHealth;
        staminaBar.fillAmount = fish.stamina / fish.maxStamina;
    }


}
