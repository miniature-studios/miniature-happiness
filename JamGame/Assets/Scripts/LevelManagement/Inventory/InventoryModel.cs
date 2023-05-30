using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventoryModel : MonoBehaviour
{
    private readonly ObservableCollection<RoomInventoryUI> roomsInInventory = new();
    public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

    public ImmutableList<RoomInventoryUI> RoomsInInventory => roomsInInventory.ToImmutableList();

    private void Awake()
    {
        roomsInInventory.CollectionChanged += CollectionChanged.Invoke;
    }

    public void AddNewRoom(RoomInventoryUI room_in_inventory)
    {
        roomsInInventory.Add(room_in_inventory);
    }

    public void RemoveRoom(RoomInventoryUI room_in_inventory)
    {
        _ = roomsInInventory.Remove(roomsInInventory.First(x => x.TileUnion == room_in_inventory.TileUnion));
    }

    public void SetRooms(IEnumerable<RoomInventoryUI> room_in_inventory)
    {
        roomsInInventory.Clear();
        foreach (RoomInventoryUI room in room_in_inventory)
        {
            roomsInInventory.Add(room);
        }
    }
}

