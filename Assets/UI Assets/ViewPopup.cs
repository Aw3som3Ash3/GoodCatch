using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PopupTest
{
    public class ViewPopup : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<ViewPopup> { }
        public ViewPopup()
        {
            
        }
    }
}
