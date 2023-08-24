using Common;
using Level;
using Level.Room;
using UnityEngine;

namespace TileBuilder
{
    [AddComponentMenu("Scripts/TileBuilder.DragAndDropAgent")]
    public class DragAndDropAgent : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private Controller controller;

        public void Hover(CoreModel coreModel)
        {
            controller.Hover(coreModel);
        }

        public void HoverLeave()
        {
            controller.OnHoverLeave();
        }

        Result<CoreModel> IDragAndDropAgent.Borrow()
        {
            return controller.Borrow();
        }

        Result IDragAndDropAgent.Drop(CoreModel coreModel)
        {
            return controller.Drop(coreModel);
        }
    }
}
