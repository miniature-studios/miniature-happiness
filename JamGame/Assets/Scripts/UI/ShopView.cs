using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopView : MonoBehaviour
{
    [SerializeField] private Transform roomsUIContainer;
    private Animator shopAnimator;

    public void OnShopRoomsChanged(List<RoomShopUI> items, NotifyCollectionChangedAction action)
    {
        switch (action)
        {
            case NotifyCollectionChangedAction.Add:
                AddNewItems(items);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveOldItems(items);
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplaceAllItems(items);
                break;
            default:
                break;
        }
    }

    private void ReplaceAllItems(List<RoomShopUI> NewItems)
    {
        foreach (RoomShopUI old_item in roomsUIContainer.transform.GetComponentsInChildren<RoomShopUI>())
        {
            Destroy(old_item.gameObject);
        }
        AddNewItems(NewItems);
    }

    private void RemoveOldItems(List<RoomShopUI> old_items)
    {
        RoomShopUI[] room_inventorys = roomsUIContainer.transform.GetComponentsInChildren<RoomShopUI>();
        foreach (RoomShopUI old_item in old_items)
        {
            Destroy(room_inventorys.First(x => x.RoomInventoryUI == old_item.RoomInventoryUI).gameObject);
        }
    }

    private void AddNewItems(List<RoomShopUI> new_items)
    {
        for (int i = 0; i < new_items.Count; i++)
        {
            new_items[i] = Instantiate(new_items[i], roomsUIContainer).GetComponent<RoomShopUI>();
        }
    }

    private void Awake()
    {
        shopAnimator = GetComponent<Animator>();
    }

    public void Open()
    {
        shopAnimator.SetBool("Showed", true);
    }

    public void Close()
    {
        shopAnimator.SetBool("Showed", false);
    }
}

