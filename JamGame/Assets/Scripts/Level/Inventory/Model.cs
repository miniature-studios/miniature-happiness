using Level.Room;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level.Inventory.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private Transform inventoryTransform;

        private ObservableCollection<CoreModel> roomsInInventory = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInInventory.CollectionChanged += CollectionChanged.Invoke;
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
            newRoom.transform.parent = inventoryTransform;
            roomsInInventory.Add(newRoom);
        }

        public CoreModel BorrowRoom(CoreModel roomInInventory)
        {
            return roomsInInventory.Remove(roomInInventory) ? roomInInventory : null;
        }

        public void Clear()
        {
            foreach (CoreModel room in roomsInInventory)
            {
                Destroy(room.gameObject);
            }
            roomsInInventory.Clear();
        }
    }
}
