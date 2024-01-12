using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    [AddComponentMenu("Scripts/CameraController/CameraController")]
    public class CameraControllerImpl : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private Vector2 moveVector = Vector2.zero;

        [SerializeField]
        private float moveSpeed = 10;

        [ReadOnly]
        [SerializeField]
        private float zoomValue = 0;

        [SerializeField]
        private float zoomSpeed = 1;

        [ReadOnly]
        [SerializeField]
        private float rotateValue = 0;

        [SerializeField]
        private float rotateSpeed = 1;

        [SerializeField]
        private Vector2 cameraSideRange = new(0, 1);

        [SerializeField]
        private Vector2 verticalArmLengthRange = new(0, 2);

        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;
        private Cinemachine3rdPersonFollow personFollow;

        private InputActions inputActions = null;

        private void Awake()
        {
            inputActions = new();
            personFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.CameraLook.Move.performed += MovePerformed;
            inputActions.CameraLook.Move.canceled += MoveCanceled;
            inputActions.CameraLook.Zoom.performed += ZoomPerformed;
            inputActions.CameraLook.Zoom.canceled += ZoomCanceled;
            inputActions.CameraLook.Rotate.performed += RotatePerformed;
            inputActions.CameraLook.Rotate.canceled += RotateCanceled;
        }

        private void OnDisable()
        {
            inputActions.CameraLook.Move.performed -= MovePerformed;
            inputActions.CameraLook.Move.canceled -= MoveCanceled;
            inputActions.CameraLook.Zoom.performed -= ZoomPerformed;
            inputActions.CameraLook.Zoom.canceled -= ZoomCanceled;
            inputActions.CameraLook.Rotate.performed -= RotatePerformed;
            inputActions.CameraLook.Rotate.canceled -= RotateCanceled;
            inputActions.Disable();
        }

        private void ZoomPerformed(InputAction.CallbackContext context)
        {
            zoomValue = context.ReadValue<float>();
        }

        private void ZoomCanceled(InputAction.CallbackContext context)
        {
            zoomValue = 0;
        }

        public void MoveCanceled(InputAction.CallbackContext context)
        {
            moveVector = Vector2.zero;
        }

        public void MovePerformed(InputAction.CallbackContext context)
        {
            moveVector = context.ReadValue<Vector2>();
        }

        private void RotateCanceled(InputAction.CallbackContext context)
        {
            rotateValue = 0;
        }

        private void RotatePerformed(InputAction.CallbackContext context)
        {
            rotateValue = context.ReadValue<float>();
        }

        private void Update()
        {
            ProcessMoving();
            bool rotated = ProcessRotation();
            if (!rotated)
            {
                ProcessZoom();
            }
        }

        private void ProcessMoving()
        {
            transform.position += transform.TransformDirection(
                Time.unscaledDeltaTime * moveSpeed * new Vector3(moveVector.y, 0, moveVector.x)
            );
        }

        private bool ProcessRotation()
        {
            if (rotateValue == 0)
            {
                return false;
            }
            transform.Rotate(
                Vector3.up,
                rotateValue * rotateSpeed * Time.unscaledDeltaTime,
                Space.Self
            );
            return true;
        }

        private void ProcessZoom()
        {
            personFollow.CameraSide = Mathf.Clamp(
                personFollow.CameraSide + (zoomValue * Time.unscaledDeltaTime * zoomSpeed),
                cameraSideRange.x,
                cameraSideRange.y
            );

            personFollow.VerticalArmLength = Mathf.Lerp(
                verticalArmLengthRange.x,
                verticalArmLengthRange.y,
                (personFollow.CameraSide - cameraSideRange.x)
                    / (cameraSideRange.y - cameraSideRange.x)
            );
        }
    }
}
