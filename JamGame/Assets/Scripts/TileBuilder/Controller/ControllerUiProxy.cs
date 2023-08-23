using Common;
using Level;
using Level.Room;
using UnityEngine;

namespace TileBuilder
{
    [AddComponentMenu("Scripts/TileBuilder.ControllerUiProxy")]
    public class ControllerUiProxy : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private Controller controller;

        public void Hover(CoreModel coreModel)
        {
            controller.Hover(coreModel);
        }

        public void HoveredOnUpdate(IDragAndDropAgent dragAndDrop)
        {
            controller.IsHoveredOnUpdate(dragAndDrop != null && dragAndDrop.GetType() == GetType());
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
