using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IHoverableToolTip<T> where T : VisualElement
{
    public Action<Action<ToolTipBox>> MouseEnter { get; }

    public Action MouseExit { get; }

    void PopulateToolTip(ToolTipBox element);
    
    public void EnableEvents()
    {
        ((T)this).RegisterCallback<MouseEnterEvent>((x) => { MouseEnter?.Invoke(PopulateToolTip); });
        ((T)this).RegisterCallback<FocusInEvent>((x) => { MouseEnter?.Invoke(PopulateToolTip); });

        ((T)this).RegisterCallback<MouseOutEvent>((x) => { MouseExit?.Invoke(); });
        ((T)this).RegisterCallback<FocusOutEvent>((x) => { MouseExit?.Invoke(); });
    }

    
}
