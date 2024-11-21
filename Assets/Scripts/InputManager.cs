using UnityEngine.InputSystem;

public static class InputManager
{
    // Start is called before the first frame update
    public static GoodCatchInputs Input { get; private set; } = new GoodCatchInputs();

    public enum InputMethod
    {
        mouseAndKeyboard,
        controller
    }
    //public static InputMethod inputMethod { get {return InputSystem. } }

    public static void EnablePlayer()
    {
        Input.Player.Enable();
    }

    public static void DisablePlayer()
    {
        Input.Player.Disable();
    }




}
