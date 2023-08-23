using Common;
using Level.Room;
using Pickle;
using System.Linq;
using UnityEngine;

namespace Level
{
    public interface IDragAndDropAgent
    {
        public void Hover(CoreModel coreModel);
        public void OnHoverLeave();
        public Result Drop(CoreModel coreModel);
        public Result<CoreModel> Borrow();
    }

    [AddComponentMenu("Scripts/Level.DragAndDropManager")]
    public class DragAndDropManager : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private CoreModel bufferCoreModel;

        [SerializeField]
        [Pickle(typeof(IDragAndDropAgent), LookupType = ObjectProviderType.Scene)]
        private GameObject backupDragAndDropProvider;
        private IDragAndDropAgent backupDragAndDrop;

        private bool mousePressed;
        private IDragAndDropAgent previousHovered;

        private void Awake()
        {
            backupDragAndDrop = backupDragAndDropProvider.GetComponent<IDragAndDropAgent>();
            if (backupDragAndDrop == null)
            {
                Debug.LogError("IDragAndDropManager not found in backupDragAndDropProvider");
            }
        }

        private void Update()
        {
            IDragAndDropAgent dragAndDrop = RayCastUtilities
                .UIRayCast(Input.mousePosition)
                ?.FirstOrDefault(x => x.GetComponent<IDragAndDropAgent>() != null)
                ?.GetComponent<IDragAndDropAgent>();

            if (Input.GetMouseButtonDown(0) && !mousePressed)
            {
                if (dragAndDrop != null)
                {
                    Result<CoreModel> result = dragAndDrop.Borrow();
                    bufferCoreModel = result.Success ? result.Data : null;
                }
                else
                {
                    bufferCoreModel = null;
                }
                mousePressed = true;
            }

            if (Input.GetMouseButtonUp(0) && mousePressed)
            {
                if (bufferCoreModel != null)
                {
                    if (dragAndDrop != null)
                    {
                        Result result = dragAndDrop.Drop(bufferCoreModel);
                        if (result.Failure)
                        {
                            BackupDrop(bufferCoreModel);
                        }
                    }
                    else
                    {
                        BackupDrop(bufferCoreModel);
                    }
                    bufferCoreModel = null;
                }
                mousePressed = false;
            }

            if (mousePressed && dragAndDrop != null && bufferCoreModel != null)
            {
                dragAndDrop.Hover(bufferCoreModel);
                if (previousHovered != dragAndDrop && previousHovered != null)
                {
                    previousHovered.OnHoverLeave();
                }
                previousHovered = dragAndDrop;
            }
            else if (previousHovered != null)
            {
                previousHovered.OnHoverLeave();
                previousHovered = null;
            }
        }

        public void BackupDrop(CoreModel coreModel)
        {
            _ = backupDragAndDrop.Drop(coreModel);
        }
    }
}
