using Common;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShopModel : MonoBehaviour
{
    private readonly List<RoomShopUI> roomsInShop = new();
    public UnityEvent<List<RoomShopUI>, NotifyCollectionChangedAction> CollectionChanged = new();

    public ImmutableList<RoomShopUI> RoomsInInventory => roomsInShop.ToImmutableList();

    public void AddNewRoom(RoomShopUI room_in_shop)
    {
        roomsInShop.Add(room_in_shop);
        CollectionChanged.Invoke(new() { room_in_shop }, NotifyCollectionChangedAction.Add);
    }

    public void RemoveRoom(RoomShopUI room_in_shop)
    {
        _ = roomsInShop.Remove(roomsInShop.First(x => x.RoomInventoryUI == room_in_shop.RoomInventoryUI));
        CollectionChanged.Invoke(new() { room_in_shop }, NotifyCollectionChangedAction.Remove);
    }

    public void SetRooms(List<RoomShopUI> room_in_inventory)
    {
        roomsInShop.Clear();
        roomsInShop.AddRange(room_in_inventory);
        CollectionChanged.Invoke(room_in_inventory, NotifyCollectionChangedAction.Replace);
    }
}

