using System.Linq;
using Common;
using Level.Room;
using UnityEngine;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level.Inventory.Controller")]
    public class Controller : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private Model model;

        public void AddNewRoom(CoreModel room)
        {
            model.AddNewRoom(room);
        }

        public void Hover(CoreModel room)
        {
            // TODO: implement another view
        }

        public Result Drop(CoreModel room)
        {
            model.AddNewRoom(room);
            return new SuccessResult();
        }

        public Result<CoreModel> Borrow()
        {
            Room.View hovered = RayCastUtilities
                .UIRayCast(Input.mousePosition)
                ?.FirstOrDefault(x => x.GetComponent<Room.View>() != null)
                ?.GetComponent<Room.View>();
            if (hovered != null)
            {
                CoreModel coreModel = hovered.CoreModel;
                return new SuccessResult<CoreModel>(model.BorrowRoom(coreModel));
            }
            else
            {
                return new FailResult<CoreModel>("Nothing hovered.");
            }
        }

        public void HoverLeave() { }
    }
}
