using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    [Serializable]
    public class NamedRoomInventoryUI
    {
        public string Name;
        public RoomInventoryUI RoomInventoryUI;
    }

    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private InventoryModel inventoryModel;
        private RoomInventoryUI selectedRoom = null;
        private bool pointerOverView = false;

        public event Func<RoomInventoryUI, Result> TryPlace;

        public List<NamedRoomInventoryUI> NamedRoomInventoryUIs;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (pointerOverView)
                {
                    selectedRoom = RaycastUtilities
                        .UIRaycast(Input.mousePosition)
                        ?.Where(x => x.GetComponent<RoomInventoryUI>())
                        ?.Select(x => x.GetComponent<RoomInventoryUI>())
                        .First();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectedRoom = null;
            }
        }

        public void AddNewRoom(RoomInventoryUI room_inventory_ui)
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

        public void JustAddedNewRoom(RoomInventoryUI new_room)
        {
            selectedRoom = new_room;
        }
    }
}
