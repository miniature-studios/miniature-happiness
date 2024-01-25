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

        private void Awake()
        {
            backupDragAndDrop = backupDragAndDropProvider.GetComponent<IDragAndDropAgent>();
            if (backupDragAndDrop == null)
            {
                Debug.LogError("IDragAndDropManager not found in backupDragAndDropProvider");
            }
            previousHovered = backupDragAndDrop;
        }

        private void Update()
        {
            Result<IDragAndDropAgent> rayCastResult = RayCastTopDragAndDropAgent();

            if (Mouse.current.leftButton.wasPressedThisFrame)
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

            if (Mouse.current.leftButton.wasReleasedThisFrame)
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

            if (
                Mouse.current.leftButton.isPressed
                && rayCastResult.Success
                && bufferCoreModel != null
            )
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
        }

        private Result<IDragAndDropAgent> RayCastTopDragAndDropAgent()
        {
            Vector2 position = Mouse.current.position.ReadValue();
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
