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
        private readonly ObservableCollection<Room.MenuView> roomsInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInShop.CollectionChanged += CollectionChanged.Invoke;
        }

        public void SetRooms(List<Room.MenuView> room_in_inventory)
        {
            roomsInShop.Clear();
            foreach (Room.MenuView room in room_in_inventory)
            {
                roomsInShop.Add(room);
            }
        }
    }
}
