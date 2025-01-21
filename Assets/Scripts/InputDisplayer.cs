using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputDisplayer 
{
    // Start is called before the first frame update
    public static Texture2D GetInputIcon(InputAction inputAction,InputMethod method)
    {
        string folder = method == InputMethod.mouseAndKeyboard ? "Keyboard & Mouse" : "Xbox Series";
        string stringMethod = GetStringMethod(method);
        var binding=inputAction.bindings.Where((x)=> InputBinding.MaskByGroup(stringMethod).Matches(x)).First();
        var control=inputAction.controls.Where((x) => 
        {
            if(x.device is Gamepad && method == InputMethod.controller)
            {
                return true;
            }else
            if(x.device is Keyboard && method == InputMethod.mouseAndKeyboard)
            {
                return true;
            }
            return false;

        }).First();
        Debug.Log("binding" + binding.effectivePath);
        Debug.Log("controls" + control.path);
       
        return Resources.Load<Texture2D>("ControlIcons/"+folder +"/Default/"+ GetPreffix(method) + control.path.Replace($"/{control.device.name}/", ""));
    }
    public static Texture2D GetInputIcon(InputBinding inputBinding, InputMethod method)
    {
        string folder = method == InputMethod.mouseAndKeyboard ? "Keyboard & Mouse" : "Xbox Series";
        return Resources.Load<Texture2D>("ControlIcons/" + folder + "/Default/"+ GetPreffix(method) + inputBinding.path.Replace($"<{GetPath(method)}>/", ""));
    }

    static string GetStringMethod(InputMethod method)
    {
        return method == InputMethod.mouseAndKeyboard ? "Keyboard&Mouse" : "Gamepad";
    }
    static string GetPath(InputMethod method)
    {
        return method == InputMethod.mouseAndKeyboard ? "Keyboard" : "Gamepad";
    }
    static string GetPreffix(InputMethod method)
    {
        return method == InputMethod.mouseAndKeyboard ? "keyboard_" : "xbox_";
    }
    
}
