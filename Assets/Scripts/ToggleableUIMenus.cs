using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ToggleableUIMenus : MonoBehaviour
{

    abstract protected InputAction input { get; }

    private void Awake()
    {
        input.performed += OpenUi;
        input.Enable();
        this.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {

    }

    void OpenUi(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
            Cursor.lockState = this.gameObject.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = this.gameObject.activeSelf;
            if (this.gameObject.activeSelf)
            {
                InputManager.DisablePlayer();
            }
            else
            {
                InputManager.EnablePlayer();
            }
            UpdateUI();

        }
    }

    abstract protected void UpdateUI();
}
