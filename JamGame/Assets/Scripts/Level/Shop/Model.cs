using Level.Room;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level.Shop.Model")]
    public class Model : MonoBehaviour
    {
        private ObservableCollection<CoreModel> roomsInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInShop.CollectionChanged += CollectionChanged.Invoke;
        }

        public void ResetRooms(IEnumerable<CoreModel> rooms)
        {
            ClearShop();
            foreach (CoreModel room in rooms)
            {
                AddRoom(room);
            }
        }

        public void AddRoom(CoreModel room)
        {
            room.transform.parent = transform;
            roomsInShop.Add(room);
        }

        public CoreModel BorrowRoom(CoreModel room)
        {
            return roomsInShop.Remove(room) ? room : null;
        }

        public void ClearShop()
        {
            foreach (CoreModel room in roomsInShop)
            {
                Destroy(room.gameObject);
            }
            roomsInShop.Clear();
        }
    }
}
