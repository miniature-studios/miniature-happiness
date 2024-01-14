using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    [AddComponentMenu("Scripts/CameraController/CameraController.Logic")]
    internal class Logic : MonoBehaviour
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

        [SerializeField]
        private TileBuilder.Controller.ControllerImpl tileController;

        private void Awake()
        {
            personFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        public void ZoomPerformed(InputAction.CallbackContext context)
        {
            zoomValue = context.ReadValue<float>();
        }

        public void ZoomCanceled(InputAction.CallbackContext context)
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

        public void RotateCanceled(InputAction.CallbackContext context)
        {
            rotateValue = 0;
        }

        public void RotatePerformed(InputAction.CallbackContext context)
        {
            rotateValue = context.ReadValue<float>();
        }

        private void Update()
        {
            bool rotated = ProcessRotation();
            if (!rotated)
            {
                ProcessMoving();
                ProcessZoom();
            }
        }

        private void ProcessMoving()
        {
            Vector3 newPosition =
                transform.position
                + transform.TransformDirection(
                    Time.unscaledDeltaTime * moveSpeed * new Vector3(moveVector.y, 0, moveVector.x)
                );

            Bounds bounds = tileController.GetBuildingBounds();
            transform.position = bounds.ClosestPoint(newPosition);
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
