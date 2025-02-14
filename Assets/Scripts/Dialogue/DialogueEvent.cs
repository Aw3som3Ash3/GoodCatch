using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueEvent : ScriptableObject
{

    public event Action @Event;
    public void Invoke()
    {
        Event?.Invoke();
    }
}
