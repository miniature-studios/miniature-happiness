using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Common;
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

        public Result<CoreModel> BorrowRoom(InternalUid roomUid)
        {
            CoreModel foundRoom = roomsInInventory.FirstOrDefault(x => x.Uid == roomUid);
            if (foundRoom != null)
            {
                _ = roomsInInventory.Remove(foundRoom);
                return new SuccessResult<CoreModel>(foundRoom);
            }
            else
            {
                return new FailResult<CoreModel>("No any CoreModel with this Uid.");
            }
        }
    }
}
