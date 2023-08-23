using Common;
using Level.Room;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public interface IDragAndDropAgent
    {
        public void Hover(CoreModel coreModel);
        public Result Drop(CoreModel coreModel);
        public Result<CoreModel> Borrow();
        public void HoveredOnUpdate(IDragAndDropAgent dragAndDrop);
    }

    [AddComponentMenu("Scripts/Level.DragAndDropManager")]
    public class DragAndDropManager : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private CoreModel bufferCoreModel;

        [SerializeField]
        [Pickle(typeof(IDragAndDropAgent))]
        private GameObject backupDragAndDropProvider;
        private IDragAndDropAgent backupDragAndDrop;

        private bool mousePressed;
        private List<IDragAndDropAgent> dragAndDropManagers;

        private void Awake()
        {
            dragAndDropManagers = FindObjectsOfType<MonoBehaviour>()
                .OfType<IDragAndDropAgent>()
                .ToList();
            backupDragAndDrop = backupDragAndDropProvider.GetComponent<IDragAndDropAgent>();
            if (backupDragAndDrop == null)
            {
                Debug.LogError("IDragAndDropManager not found in backupDragAndDropProvider");
            }
        }

        private void Update()
        {
            IDragAndDropAgent dragAndDrop = null;
            GameObject topDragAndDrop = RayCastUtilities
                .UIRayCast(Input.mousePosition)
                ?.FirstOrDefault(x => x.GetComponent<IDragAndDropAgent>() != null);
            if (topDragAndDrop != null)
            {
                dragAndDrop = topDragAndDrop.GetComponent<IDragAndDropAgent>();
            }

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
                dragAndDropManagers.ForEach(x => x.HoveredOnUpdate(dragAndDrop));
            }
            else
            {
                dragAndDropManagers.ForEach(x => x.HoveredOnUpdate(null));
            }
        }

        public void BackupDrop(CoreModel coreModel)
        {
            _ = backupDragAndDrop.Drop(coreModel);
        }
    }
}
