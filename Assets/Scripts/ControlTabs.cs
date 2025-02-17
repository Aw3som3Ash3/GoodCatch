using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControlTabs : TabbedMenu
{
    GoodCatchInputs inputs = new();
    public new class UxmlFactory : UxmlFactory<ControlTabs, UxmlTraits> { }

    public ControlTabs() 
    {
        Initial();
    }

    void Initial() 
    {

        var keyboardAndMouse= new ScrollView();
        var keyboardAndMouseTab = new TabMenuButton("Keyboard & Mouse", keyboardAndMouse);
        AddTab(keyboardAndMouseTab, true);
        var controler = new ScrollView();
        var controlerTab = new TabMenuButton("Controller", controler);
        AddTab(controlerTab, false);
        SetElements(keyboardAndMouse, InputMethod.mouseAndKeyboard);
        SetElements(controler, InputMethod.controller);
        
        inputs.UI.ChangeTab.performed += (x) => ChangeTab((int)x.ReadValue<float>());
        inputs.UI.ChangeTab.Enable();

    }

    void SetElements(ScrollView scrollView,InputMethod inputMethod)
    {
        //AddControls(scrollView, "Player", InputManager.Input.Player.Get().actions, inputMethod);
        AddControls(scrollView, "Fishing", InputManager.Input.Fishing.Get().actions, inputMethod);
        //AddControls(scrollView, "Ship", InputManager.Input.Ship.Get().actions, inputMethod);
        AddControls(scrollView, "Combat", InputManager.Input.Combat.Get().actions, inputMethod);
        //AddControls(scrollView, "UI", InputManager.Input.UI.Get().actions, inputMethod);
        

    }


    void AddControls(ScrollView scrollView,string title ,IEnumerable<InputAction> actions,InputMethod inputMethod)
    {
        scrollView.Add(SetTitleLabel(title));
        foreach (InputAction action in actions)
        {
           
            var control = new ControlsEditor(action, inputMethod);
            scrollView.Add(control);
        }
       
        //return 
    }
    Label SetTitleLabel(string title)
    {
        var label = new Label();
        label.text = title;
        label.AddToClassList("control-group-title");
        return label;
    }
    

}
