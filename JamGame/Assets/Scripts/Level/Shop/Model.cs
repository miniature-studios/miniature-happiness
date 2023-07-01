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
        private readonly ObservableCollection<Room.View> roomsInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInShop.CollectionChanged += CollectionChanged.Invoke;
        }

        public void SetRooms(List<Room.View> room_in_inventory)
        {
            roomsInShop.Clear();
            foreach (Room.View room in room_in_inventory)
            {
                roomsInShop.Add(room);
            }
        }
    }
}
