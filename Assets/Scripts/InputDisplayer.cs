using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.IO;
using System;

public static class InputDisplayer
{
    static List<IResourceLocation> location;
    static InputDisplayer()
    {
        var loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync("ControlIcons", typeof(Texture2D));
        //loadResourceLocationsHandle.Completed += (x) => { location = x.Result.ToList(); };
    }
    // Start is called before the first frame update
    public static void GetInputIcon(InputAction inputAction, InputMethod method, int index, Action<AsyncOperationHandle<Texture2D>> completed)
    {
        string folder = method == InputMethod.mouseAndKeyboard ? "Keyboard & Mouse" : "Xbox Series";
        string stringMethod = GetStringMethod(method);
        var bindings = inputAction.bindings.Where((x) => InputBinding.MaskByGroup(stringMethod).Matches(x)).ToArray();
        if (index >= bindings.Length)
        {

            return;
        }
        var binding = bindings[index];

        var controls = inputAction.controls.Where((x) =>
        {
            if (x.device is Gamepad && method == InputMethod.controller)
            {
                return true;
            }
            else
            if ((x.device is Keyboard || x.device is Mouse) && method == InputMethod.mouseAndKeyboard)
            {
                return true;
            }
            else
                return false;

        }).ToArray();

        if (index >= controls.Length)
        {

            return;
        }
        var control=controls[index];
        Debug.Log("binding" + binding.effectivePath);
        Debug.Log("controls" + control.path);
        if (location != null)
        {
            Debug.Log("pirmary key" + location[0].PrimaryKey);

        }


        var operation = Addressables.LoadAssetAsync<Texture2D>("ControlIcons/" + folder + "/Default/" + GetPreffix(control) + control.path.Replace($"/{control.device.name}/", "") + ".png");
        //var operation = Addressables.LoadAssetAsync<Texture2D>(location[0].PrimaryKey);
        operation.Completed += completed;


    }
    public static AsyncOperationHandle<Texture2D> GetInputIcon(InputBinding inputBinding, InputMethod method)
    {
        string folder = method == InputMethod.mouseAndKeyboard ? "Keyboard & Mouse" : "Xbox Series";
        var operation = Addressables.LoadAssetAsync<Texture2D>("ControlIcons/" + folder + "/Default/" + GetPreffix(method) + inputBinding.path.Replace($"<{GetPath(method)}>/", "") + ".png");

        return operation;
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
    static string GetPreffix(InputControl control)
    {
        //string s;
        if (control.device is Gamepad)
        {
            return "xbox_";
        }
        else if (control.device is Keyboard)
        {
            return "keyboard_";
        }
        else if (control.device is Mouse)
        {
            return "mouse_";
        }
        else
        {
            return "keyboard_";
        }

    }
}
