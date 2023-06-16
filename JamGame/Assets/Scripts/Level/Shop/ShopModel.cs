using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    public class ShopModel : MonoBehaviour
    {
        private readonly ObservableCollection<RoomShopUI> roomsInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        private void Awake()
        {
            roomsInShop.CollectionChanged += CollectionChanged.Invoke;
        }

        public void SetRooms(List<RoomShopUI> room_in_inventory)
        {
            roomsInShop.Clear();
            foreach (RoomShopUI room in room_in_inventory)
            {
                roomsInShop.Add(room);
            }
        }
    }
}
