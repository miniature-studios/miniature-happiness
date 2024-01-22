using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using UnityEngine;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level/Inventory/Level.Inventory.Controller")]
    public class Controller : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private Model model;

        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new InputActions();
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

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
            Result<Room.View> hovered = RayCastTopRoomView();
            if (hovered.Success)
            {
                CoreModel coreModel = hovered.Data.CoreModel;
                return new SuccessResult<CoreModel>(model.BorrowRoom(coreModel));
            }
            else
            {
                return new FailResult<CoreModel>("Nothing hovered.");
            }
        }

        public void HoverLeave() { }

        private Result<Room.View> RayCastTopRoomView()
        {
            Vector2 position = inputActions.UI.PointPosition.ReadValue<Vector2>();
            IEnumerable<GameObject> rayCastObjects = RayCastUtilities.UIRayCast(position);
            GameObject foundObject = rayCastObjects.FirstOrDefault(x =>
                x.TryGetComponent(out Room.View _)
            );
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
