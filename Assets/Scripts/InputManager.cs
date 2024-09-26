using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    // Start is called before the first frame update
    public static GoodCatchInputs Input { get; private set; } = new GoodCatchInputs();

    public static void EnablePlayer()
    {
        Input.Player.Enable();
    }

    public static void DisablePlayer()
    {
        Input.Player.Disable();
    }



}
