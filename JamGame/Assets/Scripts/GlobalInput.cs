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
        inputActions.UI.Ecape.performed += EscapePerformed;
    }

    private void EscapePerformed(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        inputActions.UI.Ecape.performed -= EscapePerformed;
        inputActions.Disable();
    }
}
