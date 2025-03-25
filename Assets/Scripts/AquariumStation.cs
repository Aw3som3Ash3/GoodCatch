using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AquariumStation : MonoBehaviour, IInteractable
{
    public string StationName => "Aquarium";

    public bool IsInteractable => true;

    public bool Interact()
    {
        //FindObjectOfType<UIDocument>().rootVisualElement.Add();
        PauseMenu.Pause().AddPage(new AquariumScreen(),true);
        return true;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
