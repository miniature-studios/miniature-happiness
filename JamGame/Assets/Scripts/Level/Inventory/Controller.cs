using Common;
using Level.Room;
using UnityEngine;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level.Inventory.Controller")]
    public class Controller : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private Model inventoryModel;

        [SerializeField]
        private View inventoryView;

        public void AddNewRoom(CoreModel room)
        {
            inventoryModel.AddNewRoom(room);
        }

        public void Hover(CoreModel coreModel)
        {
            // TODO: implement another view
        }

        public Result Drop(CoreModel coreModel)
        {
            inventoryModel.AddNewRoom(coreModel);
            return new SuccessResult();
        }

        public Result<CoreModel> Borrow()
        {
            Room.View hovered = inventoryView.GetHoveredView();
            if (hovered != null)
            {
                CoreModel coreModel = hovered.GetCoreModelInstance();
                inventoryModel.RemoveRoom(coreModel);
                return new SuccessResult<CoreModel>(coreModel);
            }
            else
            {
                return new FailResult<CoreModel>("Anything hovered.");
            }
        }

        public void OnHoverLeave() { }
    }
}
