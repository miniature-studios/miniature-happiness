﻿using Common;
using Level.Room;
using Pickle;
using System.Linq;
using UnityEngine;

namespace Level
{
    public interface IDragAndDropAgent
    {
        public void Hover(CoreModel coreModel);
        public void HoverLeave();
        public Result Drop(CoreModel coreModel);
        public Result<CoreModel> Borrow();
    }

    [AddComponentMenu("Scripts/Level.DragAndDropManager")]
    public class DragAndDropManager : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private CoreModel bufferCoreModel;

        [SerializeField]
        private Transform dragAndDropTransform;

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
        }

        private void Update()
        {
            IDragAndDropAgent dragAndDrop = RayCastUtilities
                .UIRayCast(Input.mousePosition)
                ?.FirstOrDefault(x => x.GetComponent<IDragAndDropAgent>() != null)
                ?.GetComponent<IDragAndDropAgent>();

            if (Input.GetMouseButtonDown(0))
            {
                if (dragAndDrop != null)
                {
                    Result<CoreModel> result = dragAndDrop.Borrow();
                    if (result.Success)
                    {
                        bufferCoreModel = result.Data;
                        bufferCoreModel.transform.parent = dragAndDropTransform;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (
                    bufferCoreModel != null
                    && dragAndDrop != null
                    && dragAndDrop.Drop(bufferCoreModel).Failure
                )
                {
                    _ = backupDragAndDrop.Drop(bufferCoreModel);
                }
                bufferCoreModel = null;
            }

            if (Input.GetMouseButton(0) && dragAndDrop != null && bufferCoreModel != null)
            {
                dragAndDrop.Hover(bufferCoreModel);
                if (previousHovered != dragAndDrop && previousHovered != null)
                {
                    previousHovered.HoverLeave();
                }
                previousHovered = dragAndDrop;
            }
            else if (previousHovered != null)
            {
                previousHovered.HoverLeave();
                previousHovered = null;
            }
        }
    }
}
