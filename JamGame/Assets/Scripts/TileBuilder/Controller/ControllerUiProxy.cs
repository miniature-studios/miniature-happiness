using Common;
using Level;
using Level.Room;
using UnityEngine;

namespace TileBuilder
{
    public class ControllerUiProxy : MonoBehaviour, IDragAndDropManager
    {
        [SerializeField] private Controller controller;

        public void Hover(CoreModel coreModel)
        {
            controller.Hover(coreModel);
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
