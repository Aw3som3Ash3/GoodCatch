using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartyUI : MonoBehaviour
{
    Fishventory fishventory;
    [SerializeField]
    PartyIcon[] partyIcon;

    GoodCatchInputs input;
    public GameObject partyPanel;

    // Start is called before the first frame update
    void Start()
    {
        fishventory = GameManager.Instance.playerFishventory;

        input = new GoodCatchInputs();
        input.UI.Party.Enable();
        input.UI.Party.performed += Party;

        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        for (int i = 0; i < fishventory.Fishies.Count; i++)
        {
            partyIcon[i].SetIcon(fishventory.Fishies[i].Icon, Color.cyan);
            Debug.Log(partyIcon[i]);
        }
    }

    private void Party(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            partyPanel.SetActive(!partyPanel.activeSelf);
            UpdateUI();
        }
    }
}