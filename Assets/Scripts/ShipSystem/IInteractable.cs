using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool Interactable()
    {
        return true ;
    
    }
    public bool Interact();

    public string StationName();
}
