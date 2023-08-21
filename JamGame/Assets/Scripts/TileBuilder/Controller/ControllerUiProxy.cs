using Common;
using Level;
using Level.Room;
using UnityEngine;

namespace TileBuilder
{
    [AddComponentMenu("Scripts/TileBuilder.ControllerUiProxy")]
    public class ControllerUiProxy : MonoBehaviour, IDragAndDropManager
    {
        [SerializeField]
        private Controller controller;

        public void Hover(CoreModel coreModel)
        {
            controller.Hover(coreModel);
        }

        public void HoveredOnUpdate(IDragAndDropManager dragAndDrop)
        {
            controller.IsHoveredOnUpdate(dragAndDrop != null && dragAndDrop.GetType() == GetType());
        }

        Result<CoreModel> IDragAndDropManager.Borrow()
        {
            return controller.Borrow();
        }

        Result IDragAndDropManager.Drop(CoreModel coreModel)
        {
            return controller.Drop(coreModel);
        }
    }
}
