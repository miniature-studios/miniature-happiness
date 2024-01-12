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
        private float moveSpeed;

        private InputActions inputActions = null;

        private void Awake()
        {
            inputActions = new();
            inputActions.Enable();
            inputActions.CameraLook.Move.performed += MovePerformed;
            inputActions.CameraLook.Move.canceled += MoveCanceled;
        }

        public void MoveCanceled(InputAction.CallbackContext context)
        {
            moveVector = Vector2.zero;
        }

        public void MovePerformed(InputAction.CallbackContext context)
        {
            moveVector = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            Debug.Log(transform.position);
            transform.position += transform.TransformDirection(
                new Vector3(moveVector.y, 0, moveVector.x) * Time.unscaledDeltaTime * moveSpeed
            );
        }
    }
}
