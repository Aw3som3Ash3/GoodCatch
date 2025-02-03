using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public class ControlsEditor : VisualElement
{
    public string inputName { get; set; }
    Button primary, alt;
    
    Label title;
    InputMethod inputMethod { get; set; }

    public new class UxmlFactory : UxmlFactory<ControlsEditor, ControlsEditor.UxmlTraits>
    {

    }
    public new class UxmlTraits : UnityEngine.UIElements.UxmlTraits
    {
        UxmlStringAttributeDescription m_inputName = new UxmlStringAttributeDescription { name = "input-Name", defaultValue = "" };
        UxmlEnumAttributeDescription<InputMethod> m_inputMethod = new(){ name="input-method",defaultValue=InputMethod.mouseAndKeyboard};
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as ControlsEditor;
            ate.inputMethod = m_inputMethod.GetValueFromBag(bag,cc);
            ate.inputName = m_inputName.GetValueFromBag(bag, cc);
            ate.title.text = m_inputName.GetValueFromBag(bag, cc);
            ate.ChangeIcons();
        }

    }
    public ControlsEditor()
    {
        Initial();
    }

    void ChangeIcons()
    {
        InputAction inputAction=null;
        if (inputName != null)
        {
            inputAction = InputManager.Input.FindAction(inputName);
            Debug.Log(inputAction);
        }

        if (inputAction != null)
        {
            InputDisplayer.GetInputIcon(inputAction, inputMethod,0, (x) => primary.style.backgroundImage = x.Result);
            InputDisplayer.GetInputIcon(inputAction, inputMethod,1, (x) => alt.style.backgroundImage = x.Result);
            Debug.Log("has changed background");
        }
    }
    private void Initial()
    {
        VisualElement root = this;
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UXMLs/SubModControl");

        visualTreeAsset.CloneTree(root);


        title = this.Q<Label>("control-title");
        primary = this.Q<Button>("Primary");
        alt = this.Q<Button>("Alternative");
        primary.text = "";
        primary.style.width = 60;
        primary.style.height = 60;
        primary.clicked += () => ChangeInput(0);
        alt.text = "";
        alt.style.width = 60;
        alt.style.height = 60;
        alt.clicked += () => ChangeInput(1);


        //title.bindingPath = "inputName";
        //throw new NotImplementedException();
    }

    void ChangeInput(int index)
    {
        var inputAction = InputManager.Input.FindAction(inputName);
        string stringMethod = inputMethod == InputMethod.mouseAndKeyboard ? "Keyboard&Mouse" : "Gamepad";
        var bindings = inputAction.bindings.Where((x) => InputBinding.MaskByGroup(stringMethod).Matches(x)).ToArray();
        
        //InputManager.Input.
        if (index >= bindings.Length)
        {
           
            InputSystem.onAnyButtonPress.CallOnce((x) => 
            { 
                var binding=inputAction.AddBinding(x.path); 
                binding.WithGroup(stringMethod);
                Debug.Log(binding); 
                ChangeIcons(); 
            });
        }
        else
        {
            var bindingIndex = inputAction.GetBindingIndex(bindings[index]);
            InputSystem.onAnyButtonPress.CallOnce((x) => { inputAction.ApplyBindingOverride(bindingIndex, x.path); ChangeIcons(); InputHudTip.UpdateAllIcons(); });
        }

    }


}
