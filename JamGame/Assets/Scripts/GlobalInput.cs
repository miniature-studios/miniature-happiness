using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalInput : MonoBehaviour
{
    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Pause.performed += PausePerformed;
    }

    private void PausePerformed(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        inputActions.UI.Pause.performed -= PausePerformed;
        inputActions.Disable();
    }
}
