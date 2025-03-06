using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public static class InputManager
{
    //static bool initiated;
    // Start is called before the first frame update
    public static GoodCatchInputs Input { get; private set; } = new();
    public static InputMethod inputMethod { get; private set; }
    static InputUser user;
    static public event Action<InputMethod> OnInputChange;




    //public static InputMethod inputMethod { get {return InputSystem. } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        //if (Input != null)
        //{
        //    InputUser.onUnpairedDeviceUsed -= OnDeviceChange;
        //    InputUser.onChange -= OnDeviceChange;
        //    Input.Dispose();
            
        //}
        //Input = new GoodCatchInputs();

        //initiated = true;
        user = InputUser.CreateUserWithoutPairedDevices();
        InputUser.listenForUnpairedDeviceActivity = 1;
        InputUser.onUnpairedDeviceUsed += OnDeviceChange;
        InputUser.onChange += OnDeviceChange;
        
    }

    
    public static void EnablePlayer()
    {
        Input.Player.Enable();
    }

    public static void DisablePlayer()
    {
        Input.Player.Disable();
    }
    public static void EnableCombat()
    {
        Input.Combat.Enable();
        Input.Player.Disable();
    }

    public static void DisableCombat()
    {
        Input.Combat.Disable();
        
    }

    private static void OnDeviceChange(InputControl control, InputEventPtr ptr)
    {
        Debug.Log(control.device);
        //user.UnpairDevices();


        if ((control.device is Mouse || control.device is Keyboard) && (user.pairedDevices.FirstOrDefault((x) => x is Mouse || x is Keyboard) != null))
        {

            InputUser.PerformPairingWithDevice(control.device, user);
        }
        else
        {
            InputUser.PerformPairingWithDevice(control.device, user, InputUserPairingOptions.UnpairCurrentDevicesFromUser);

        }

        //throw new NotImplementedException();
    }

    private static void OnDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {

        Debug.Log(user);
        Debug.Log(device);
        if (change == InputUserChange.DevicePaired)
        {

            if (device is Gamepad)
            {
                Debug.Log("is gamepad");
                inputMethod = InputMethod.controller;
            }
            else if (device is Mouse || device is Keyboard)
            {
                inputMethod = InputMethod.mouseAndKeyboard;
                Debug.Log("is m&k");
            }
            OnInputChange?.Invoke(inputMethod);
        }
        else if (change == InputUserChange.Removed)
        {
            //user.UnpairDevice(device);
        }

    }

  
}
public enum InputMethod
{
    mouseAndKeyboard,
    controller
}

