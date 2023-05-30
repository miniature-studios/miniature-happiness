using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShopModel : MonoBehaviour
{
    private readonly ObservableCollection<RoomShopUI> roomsInShop = new();
    public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

    public ImmutableList<RoomShopUI> RoomsInInventory => roomsInShop.ToImmutableList();

    private void Awake()
    {
        roomsInShop.CollectionChanged += CollectionChanged.Invoke;
    }

    public void AddNewRoom(RoomShopUI room_in_shop)
    {
        roomsInShop.Add(room_in_shop);
    }

    public void RemoveRoom(RoomShopUI room_in_shop)
    {
        _ = roomsInShop.Remove(roomsInShop.First(x => x.RoomInventoryUI == room_in_shop.RoomInventoryUI));
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

