using UnityEngine;

namespace CameraController
{
    [AddComponentMenu("Scripts/CameraController/CameraController.Input")]
    internal class Input : MonoBehaviour
    {
        [SerializeField]
        private Logic logic;

        private InputActions inputActions = null;

        private void Awake()
        {
            inputActions = new();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.CameraLook.Move.performed += logic.MovePerformed;
            inputActions.CameraLook.Move.canceled += logic.MoveCanceled;
            inputActions.CameraLook.Zoom.performed += logic.ZoomPerformed;
            inputActions.CameraLook.Zoom.canceled += logic.ZoomCanceled;
            inputActions.CameraLook.Rotate.performed += logic.RotatePerformed;
            inputActions.CameraLook.Rotate.canceled += logic.RotateCanceled;
        }

        private void OnDisable()
        {
            inputActions.CameraLook.Move.performed -= logic.MovePerformed;
            inputActions.CameraLook.Move.canceled -= logic.MoveCanceled;
            inputActions.CameraLook.Zoom.performed -= logic.ZoomPerformed;
            inputActions.CameraLook.Zoom.canceled -= logic.ZoomCanceled;
            inputActions.CameraLook.Rotate.performed -= logic.RotatePerformed;
            inputActions.CameraLook.Rotate.canceled -= logic.RotateCanceled;
            inputActions.Disable();
        }
    }
}
