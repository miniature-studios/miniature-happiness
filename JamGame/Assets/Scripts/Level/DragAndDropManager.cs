using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using TNRD;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Raycast;

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
        private SerializableInterface<IDragAndDropAgent> backupDragAndDrop;
        private IDragAndDropAgent BackupDragAndDrop => backupDragAndDrop.Value;
        private IDragAndDropAgent previousHovered;

        private void Awake()
        {
            previousHovered = BackupDragAndDrop;
        }

        private void Update()
        {
            Result<IDragAndDropAgent> rayсastResult = RayCastTopDragAndDropAgent();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (rayсastResult.Success)
                {
                    Result<CoreModel> result = rayсastResult.Data.Borrow();
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
                    && rayсastResult.Success
                    && rayсastResult.Data.Drop(bufferCoreModel).Failure
                )
                {
                    _ = BackupDragAndDrop.Drop(bufferCoreModel);
                }
                bufferCoreModel = null;
            }

            if (
                Mouse.current.leftButton.isPressed
                && rayсastResult.Success
                && bufferCoreModel != null
            )
            {
                rayсastResult.Data.Hover(bufferCoreModel);
                if (previousHovered != rayсastResult.Data && previousHovered != null)
                {
                    previousHovered.HoverLeave();
                }
                previousHovered = rayсastResult.Data;
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
            IEnumerable<GameObject> hits = RayCaster.UIRayCast(position);
            GameObject foundObject = hits.FirstOrDefault(x =>
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
