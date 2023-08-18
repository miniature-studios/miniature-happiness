using Common;
using Level.Room;
using System.Linq;
using UnityEngine;

namespace Level
{
    public interface IDragAndDropManager
    {
        public void Hover(CoreModel coreModel);
        public Result Drop(CoreModel coreModel);
        public Result<CoreModel> Borrow();
    }

    public class DragAndDropManager : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private CoreModel bufferCoreModel;
        private IDragAndDropManager dragAndDrop;
        private bool mousePressed;

        private void Update()
        {
            GameObject topDragAndDrop = RayCastUtilities
                .UIRayCast(Input.mousePosition)
                .First(x => x.GetComponent<IDragAndDropManager>() != null);
            if (topDragAndDrop != null)
            {
                dragAndDrop = topDragAndDrop.GetComponent<IDragAndDropManager>();
            }

            if (Input.GetMouseButtonDown(0) && dragAndDrop != null)
            {
                if (!mousePressed)
                {
                    bufferCoreModel = dragAndDrop.Borrow();
                }
                mousePressed = true;
            }

            if (Input.GetMouseButtonUp(0) && dragAndDrop != null)
            {
                if (mousePressed)
                {
                    _ = dragAndDrop.Drop(bufferCoreModel);
                }
                mousePressed = false;
            }

            if (mousePressed)
            {
                dragAndDrop.Hover(bufferCoreModel);
            }
        }
    }
}
