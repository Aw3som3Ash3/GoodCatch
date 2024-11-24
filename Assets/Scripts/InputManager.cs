using UnityEngine.InputSystem;

public static class InputManager
{
    // Start is called before the first frame update
    public static GoodCatchInputs Input { get; private set; } = new GoodCatchInputs();

  
    //public static InputMethod inputMethod { get {return InputSystem. } }

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



}
public enum InputMethod
{
    mouseAndKeyboard,
    controller
}