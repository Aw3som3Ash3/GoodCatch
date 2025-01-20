using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class InputHudTip : VisualElement
{
    
    public string inputName { get; set; }
    InputAction inputAction;
    public new class UxmlFactory : UxmlFactory<InputHudTip, UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {
        UxmlStringAttributeDescription m_inputName = new UxmlStringAttributeDescription {name="inputName", defaultValue="" };
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as InputHudTip;
            ate.inputName = m_inputName.GetValueFromBag(bag, cc);
            ate.ChangeIcon();
            //ate.SubscribeToInputChange();
        }
    }


    public InputHudTip()
    {
        Init();

    }

    public void Init()
    {
        if (GameManager.Instance!= null)
        {
            ChangeIcon(GameManager.Instance.inputMethod);
        }
        else
        {
            ChangeIcon();
        }
       
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnInputChange += ChangeIcon;
            Debug.Log("subscribed to input chnage");
        }
    }
    void ChangeIcon(InputMethod inputMethod=InputMethod.mouseAndKeyboard)
    {
        if (inputName != null)
        {
            inputAction = InputManager.Input.FindAction(inputName);
            Debug.Log(inputAction);
        }

        if (inputAction != null)
        {
            this.style.backgroundImage = InputDisplayer.GetInputIcon(inputAction, inputMethod);
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
