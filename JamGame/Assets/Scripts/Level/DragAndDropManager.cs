using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Level
{
    public interface IDragAndDropAgent
    {
        public void Hover(CoreModel coreModel);
        public void HoverLeave();
        public Result Drop(CoreModel coreModel);
        public Result<CoreModel> Borrow();
    }

    [AddComponentMenu("Scripts/Level/Level.DragAndDropManager")]
    public class DragAndDropManager : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private CoreModel bufferCoreModel;

        [SerializeField]
        [Pickle(typeof(IDragAndDropAgent), LookupType = ObjectProviderType.Scene)]
        private GameObject backupDragAndDropProvider;
        private IDragAndDropAgent backupDragAndDrop;
        private IDragAndDropAgent previousHovered;

        private InputActions inputActions;

        [ReadOnly]
        [SerializeField]
        private bool isPointLeftClicked = false;

        [ReadOnly]
        [SerializeField]
        private bool isPointLeftHeld = false;

        [ReadOnly]
        [SerializeField]
        private bool isPointLeftReleased = false;

        private void Awake()
        {
            backupDragAndDrop = backupDragAndDropProvider.GetComponent<IDragAndDropAgent>();
            if (backupDragAndDrop == null)
            {
                Debug.LogError("IDragAndDropManager not found in backupDragAndDropProvider");
            }
            previousHovered = backupDragAndDrop;
            inputActions = new InputActions();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.UI.PointerLeftClick.performed += PointLeftClickPerformed;
            inputActions.UI.PointerLeftClick.canceled += PointLeftClickCanceled;
        }

        private void PointLeftClickPerformed(InputAction.CallbackContext context)
        {
            isPointLeftClicked = true;
        }

        private void PointLeftClickCanceled(InputAction.CallbackContext context)
        {
            isPointLeftReleased = true;
        }

        private void OnDisable()
        {
            inputActions.UI.PointerLeftClick.performed -= PointLeftClickPerformed;
            inputActions.UI.PointerLeftClick.canceled -= PointLeftClickCanceled;
            inputActions.Disable();
        }

        private void Update()
        {
            Result<IDragAndDropAgent> rayCastResult = RayCastTopDragAndDropAgent();

            if (isPointLeftClicked)
            {
                if (rayCastResult.Success)
                {
                    Result<CoreModel> result = rayCastResult.Data.Borrow();
                    if (result.Success)
                    {
                        bufferCoreModel = result.Data;
                        bufferCoreModel.transform.SetParent(transform);
                    }
                }
            }

            if (isPointLeftReleased)
            {
                if (
                    bufferCoreModel != null
                    && rayCastResult.Success
                    && rayCastResult.Data.Drop(bufferCoreModel).Failure
                )
                {
                    _ = backupDragAndDrop.Drop(bufferCoreModel);
                }
                bufferCoreModel = null;
            }

            if (isPointLeftHeld && rayCastResult.Success && bufferCoreModel != null)
            {
                rayCastResult.Data.Hover(bufferCoreModel);
                if (previousHovered != rayCastResult.Data && previousHovered != null)
                {
                    previousHovered.HoverLeave();
                }
                previousHovered = rayCastResult.Data;
            }
            else if (previousHovered != null)
            {
                previousHovered.HoverLeave();
                previousHovered = null;
            }

            if (isPointLeftClicked)
            {
                isPointLeftHeld = true;
            }
            if (isPointLeftReleased)
            {
                isPointLeftHeld = false;
            }
            isPointLeftClicked = false;
            isPointLeftReleased = false;
        }

        private Result<IDragAndDropAgent> RayCastTopDragAndDropAgent()
        {
            Vector2 position = inputActions.UI.PointerPosition.ReadValue<Vector2>();
            IEnumerable<GameObject> rayCastObjects = RayCastUtilities.UIRayCast(position);
            GameObject foundObject = rayCastObjects.FirstOrDefault(x =>
                x.TryGetComponent(out IDragAndDropAgent _)
            );
            if (foundObject != null)
            {
                return new SuccessResult<IDragAndDropAgent>(
                    foundObject.GetComponent<IDragAndDropAgent>()
                );
            }
            else
            {
                return new FailResult<IDragAndDropAgent>("IDragAndDropAgent not found");
            }
        }
    }
}
