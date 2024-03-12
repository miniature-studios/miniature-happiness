using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Raycast;

namespace Level.Inventory.Controller
{
    [AddComponentMenu("Scripts/Level/Inventory/Controller/Level.Inventory.Controller")]
    public partial class ControllerImpl : MonoBehaviour, IDragAndDropAgent
    {
        [Required]
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
            Result<Room.View> hoveredResult = RayCastTopRoomView();
            if (hoveredResult.Success)
            {
                InternalUid hoveredUid = hoveredResult.Data.Uid;
                Result<CoreModel> borrowedResult = model.BorrowRoom(hoveredUid);
                if (borrowedResult.Success)
                {
                    return new SuccessResult<CoreModel>(borrowedResult.Data);
                }
                else
                {
                    return borrowedResult;
                }
            }
            else
            {
                return new FailResult<CoreModel>("Nothing hovered.");
            }
        }

        public void HoverLeave() { }

        private Result<Room.View> RayCastTopRoomView()
        {
            Vector2 position = Mouse.current.position.ReadValue();
            IEnumerable<GameObject> hits = RayCaster.UIRayCast(position);
            GameObject foundObject = hits.FirstOrDefault(x => x.TryGetComponent(out Room.View _));
            if (foundObject != null)
            {
                return new SuccessResult<Room.View>(foundObject.GetComponent<Room.View>());
            }
            else
            {
                return new FailResult<Room.View>("Room.View not found");
            }
        }
    }
}
