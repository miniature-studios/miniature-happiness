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

        public void ResetRooms(List<CoreModel> rooms)
        {
            ClearShop();
            foreach (CoreModel room in rooms)
            {
                AddRoom(room);
            }
        }

        public void AddRoom(CoreModel room)
        {
            roomsInShop.Add(room);
        }

        public void RemoveRoom(CoreModel room)
        {
            _ = roomsInShop.Remove(room);
        }

        public void ClearShop()
        {
            roomsInShop.Clear();
        }
    }
}
