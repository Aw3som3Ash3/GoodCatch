using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class InputHudTip : VisualElement
{

    public string inputName { get; set; }
    InputAction inputAction;

    static event Action UpdateIcons;
    public new class UxmlFactory : UxmlFactory<InputHudTip, UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {
        UxmlStringAttributeDescription m_inputName = new UxmlStringAttributeDescription { name = "inputName", defaultValue = "" };
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as InputHudTip;
            ate.inputName = m_inputName.GetValueFromBag(bag, cc);
            ate.ChangeIcon(InputManager.inputMethod);
            

        }
    }

    public InputHudTip()
    {
        UpdateIcons += () => ChangeIcon();
        Init();

    }
    public InputHudTip(InputAction inputAction)
    {
        Init();

    }
    public static void UpdateAllIcons()
    {
        UpdateIcons?.Invoke();
    }
    public void Init()
    {
        ChangeIcon(InputManager.inputMethod);
        InputManager.OnInputChange += ChangeIcon;

    }
    void ChangeIcon(InputMethod inputMethod = InputMethod.mouseAndKeyboard)
    {
        if (inputName != null)
        {
            inputAction = InputManager.Input.FindAction(inputName);
            Debug.Log(inputAction);
        }

        if (inputAction != null)
        {
            InputDisplayer.GetInputIcon(inputAction, inputMethod, 0, (x) => this.style.backgroundImage = x.Result);
            Debug.Log("has changed background");
        }
    }
    //~InputHudTip()
    //{
    //    if (GameManager.Instance != null)
    //    {
    //        GameManager.Instance.OnInputChange -= ChangeIcon;
    //    }
    //}

}
