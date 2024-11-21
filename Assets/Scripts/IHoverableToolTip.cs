using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IHoverableToolTip<T> where T : VisualElement
{
    public event Action<Action<ToolTipBox>> MouseEnter;

    public event Action MouseExit;

    void PopulateToolTip(ToolTipBox element);

    
}
