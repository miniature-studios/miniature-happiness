using Level.Room;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level.Inventory.Model")]
    public class Model : MonoBehaviour
    {
        private ObservableCollection<CoreModel> roomsInInventory = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInInventory.CollectionChanged += CollectionChanged.Invoke;
        }

        public int GetModelsCount(CoreModel coreModel)
        {
            return roomsInInventory.Where(x => x == coreModel).Count();
        }

        public void ResetRooms(List<CoreModel> newRooms)
        {
            Clear();
            foreach (CoreModel room in newRooms)
            {
                AddNewRoom(room);
            }
        }

        public void AddNewRoom(CoreModel newRoom)
        {
            roomsInInventory.Add(newRoom);
        }

        public void RemoveRoom(CoreModel roomInInventory)
        {
            _ = roomsInInventory.Remove(roomInInventory);
        }

        public void Clear()
        {
            roomsInInventory.Clear();
        }
    }
}
