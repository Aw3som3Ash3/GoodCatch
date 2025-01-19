using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputDisplayer 
{
    // Start is called before the first frame update
    public static Texture2D GetInputIcon(InputAction inputAction,InputMethod method)
    {
        string folder = method == InputMethod.mouseAndKeyboard ? "Keyboard & Mouse" : "Xbox Series";
        string stringMethod = GetStringMethod(method);
        var binding=inputAction.bindings.Where((x)=>x.groups==stringMethod).First();
        Debug.Log("binding" + binding);
        return Resources.Load<Texture2D>("ControlIcons/"+folder +"/Default/"+ GetPreffix(method) + binding.path.Replace($"<{GetPath(method)}>/", ""));
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
