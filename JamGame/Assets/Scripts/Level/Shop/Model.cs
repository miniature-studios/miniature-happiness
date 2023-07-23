using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Shop
{
    [AddComponentMenu("Level.Shop.Model")]
    public class Model : MonoBehaviour
    {
        private readonly ObservableCollection<Room.Model> roomsInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInShop.CollectionChanged += CollectionChanged.Invoke;
        }

        public void SetRooms(List<Room.Model> rooms)
        {
            roomsInShop.Clear();
            foreach (Room.Model room in rooms)
            {
                roomsInShop.Add(room);
            }
        }
    }
}
