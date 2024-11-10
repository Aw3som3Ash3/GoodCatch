using UnityEngine;
using UnityEngine.InputSystem;

public class PartyUI : ToggleableUIMenus
{
    Fishventory fishventory;
    [SerializeField]
    PartyIcon[] partyIcon;
    public GameObject partyPanel;

    protected override InputAction input => InputManager.Input.UI.Party;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void UpdateUI()
    {
        fishventory = GameManager.Instance.PlayerFishventory;
        for (int i = 0; i < fishventory.Fishies.Count; i++)
        {
            //partyIcon[i].SetIcon(fishventory.Fishies[i].Icon, Color.cyan);
            Debug.Log(partyIcon[i]);
        }
    }


}