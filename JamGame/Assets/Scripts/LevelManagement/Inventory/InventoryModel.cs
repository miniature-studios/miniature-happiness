using Common;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventoryModel : MonoBehaviour
{
    private readonly List<RoomInventoryUI> roomsInInventory = new();
    public UnityEvent<List<RoomInventoryUI>, NotifyCollectionChangedAction> CollectionChanged = new();

    public ImmutableList<RoomInventoryUI> RoomsInInventory => roomsInInventory.ToImmutableList();

    public void AddNewRoom(RoomInventoryUI room_in_inventory)
    {
        roomsInInventory.Add(room_in_inventory);
        CollectionChanged.Invoke(new() { room_in_inventory }, NotifyCollectionChangedAction.Add);
    }

    public void RemoveRoom(RoomInventoryUI room_in_inventory)
    {
        _ = roomsInInventory.Remove(roomsInInventory.First(x => x.TileUnion == room_in_inventory.TileUnion));
        CollectionChanged.Invoke(new() { room_in_inventory }, NotifyCollectionChangedAction.Remove);
    }

    public void SetRooms(List<RoomInventoryUI> room_in_inventory)
    {
        roomsInInventory.Clear();
        roomsInInventory.AddRange(room_in_inventory);
        CollectionChanged.Invoke(room_in_inventory, NotifyCollectionChangedAction.Replace);
    }
}

