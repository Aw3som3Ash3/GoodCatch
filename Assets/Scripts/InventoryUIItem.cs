using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUIItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameText, typeText, amountText;



    public void SetValues(string name,string type, int amount)
    {
        nameText.text = name;
        typeText.text = type;
        amountText.text = amount.ToString();
    }
}
