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
            throw new System.NotImplementedException();
        }

        Result<CoreModel> IDragAndDropManager.Borrow()
        {
            throw new System.NotImplementedException();
        }

        Result IDragAndDropManager.Drop(CoreModel coreModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
