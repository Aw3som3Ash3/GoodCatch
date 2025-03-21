using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PopupTest
{
    public class ViewPopup : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<ViewPopup> { }

        private const string styleResource = "UXMLs/PopupWindowStyleSheet";
        private const string ussPopup = "popup_window";
        private const string ussPopupContainer = "popup_container";

        public ViewPopup()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
            AddToClassList(ussPopupContainer);

            VisualElement window = new VisualElement();
            window.AddToClassList(ussPopup);
            hierarchy.Add(window);
        }
    }
}
