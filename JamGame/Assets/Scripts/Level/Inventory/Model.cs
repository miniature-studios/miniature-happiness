using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Level.Room;
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
