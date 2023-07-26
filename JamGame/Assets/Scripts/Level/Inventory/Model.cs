using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Inventory
{
    [AddComponentMenu("Level.Inventory.Model")]
    public class Model : MonoBehaviour
    {
        private ObservableCollection<Room.Model> roomsInInventory = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInInventory.CollectionChanged += CollectionChanged.Invoke;
        }

        public void AddNewRoom(Room.Model room_in_inventory)
        {
            roomsInInventory.Add(room_in_inventory);
        }

        public void RemoveRoom(Room.Model room_in_inventory)
        {
            _ = roomsInInventory.Remove(
                roomsInInventory.First(x => x.TileUnion == room_in_inventory.TileUnion)
            );
        }
    }
}
