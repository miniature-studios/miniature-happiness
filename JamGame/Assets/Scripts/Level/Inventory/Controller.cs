using Common;
using System;
using System.Linq;
using UnityEngine;

namespace Level.Inventory
{
    [AddComponentMenu("Level.Inventory.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private Model inventoryModel;

        [SerializeField, HideInInspector]
        private Room.Model selectedRoom = null;

        [SerializeField, HideInInspector]
        private bool pointerOverView = false;

        public event Func<Room.Model, Result> TryPlace;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (pointerOverView)
                {
                    selectedRoom = RaycastUtilities
                        .UIRaycast(Input.mousePosition)
                        ?.Where(x => x.GetComponent<Room.Model>())
                        ?.Select(x => x.GetComponent<Room.Model>())
                        .First();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectedRoom = null;
            }
        }

        public void AddNewRoom(Room.Model room)
        {
            inventoryModel.AddNewRoom(room);
        }

        public void PointerOverView(bool over)
        {
            pointerOverView = over;
            if (!over && selectedRoom != null)
            {
                if (TryPlace(selectedRoom).Success)
                {
                    inventoryModel.RemoveRoom(selectedRoom);
                }
                selectedRoom = null;
            }
        }

        public void JustAddedNewRoom(Room.Model new_room)
        {
            selectedRoom = new_room;
        }
    }
}
