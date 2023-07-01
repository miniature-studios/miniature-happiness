using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Inventory
{
    [Serializable]
    public class NamedRoomInventoryUI
    {
        public string Name;
        public Room.View RoomInventoryUI;
    }

    [AddComponentMenu("Level.Inventory.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private Model inventoryModel;
        private Room.View selectedRoom = null;
        private bool pointerOverView = false;

        public event Func<Room.View, Result> TryPlace;

        public List<NamedRoomInventoryUI> NamedRoomInventoryUIs;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (pointerOverView)
                {
                    selectedRoom = RaycastUtilities
                        .UIRaycast(Input.mousePosition)
                        ?.Where(x => x.GetComponent<Room.View>())
                        ?.Select(x => x.GetComponent<Room.View>())
                        .First();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectedRoom = null;
            }
        }

        public void AddNewRoom(Room.View room_inventory_ui)
        {
            inventoryModel.AddNewRoom(room_inventory_ui);
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

        public void JustAddedNewRoom(Room.View new_room)
        {
            selectedRoom = new_room;
        }
    }
}
