using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Level.Room;
using UnityEngine;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level/Inventory/Level.Inventory.Model")]
    public class Model : MonoBehaviour
    {
        private ObservableCollection<CoreModel> roomsInInventory = new();
        public event NotifyCollectionChangedEventHandler InventoryRoomsCollectionChanged
        {
            add => roomsInInventory.CollectionChanged += value;
            remove => roomsInInventory.CollectionChanged -= value;
        }

        public void AddNewRoom(CoreModel newRoom)
        {
            roomsInInventory.Add(newRoom);
        }

        public CoreModel BorrowRoom(CoreModel roomInInventory)
        {
            return roomsInInventory.Remove(roomInInventory) ? roomInInventory : null;
        }
    }
}
