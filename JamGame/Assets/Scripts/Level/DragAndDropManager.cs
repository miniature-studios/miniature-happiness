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
        private bool isPointerClicked;
        private bool isPointerCanceled;

        private void Awake()
        {
            backupDragAndDrop = backupDragAndDropProvider.GetComponent<IDragAndDropAgent>();
            if (backupDragAndDrop == null)
            {
                Debug.LogError("IDragAndDropManager not found in backupDragAndDropProvider");
            }
            inputActions = new InputActions();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.UI.PointLeftClick.performed += ClickPerformed;
            inputActions.UI.PointLeftClick.canceled += ClickCanceled;
        }

        private void ClickPerformed(InputAction.CallbackContext context)
        {
            isPointerClicked = true;
        }

        private void ClickCanceled(InputAction.CallbackContext context)
        {
            isPointerCanceled = true;
        }

        private void OnDisable()
        {
            inputActions.UI.PointLeftClick.canceled -= ClickCanceled;
            inputActions.UI.PointLeftClick.performed -= ClickPerformed;
            inputActions.Disable();
        }

        private void Update()
        {
            Result<IDragAndDropAgent> rayCastResult = RayCastFirstAgent();

            if (isPointerClicked)
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

            if (isPointerCanceled)
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

            if (isPointerClicked && rayCastResult.Success && bufferCoreModel != null)
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

            if (isPointerCanceled)
            {
                isPointerClicked = false;
            }
            isPointerCanceled = false;
        }

        private Result<IDragAndDropAgent> RayCastFirstAgent()
        {
            Vector2 position = inputActions.UI.PointPosition.ReadValue<Vector2>();
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
