using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class ShopView : MonoBehaviour
{
    [SerializeField] private Transform roomsUIContainer;
    private Animator shopAnimator;

    public void OnShopRoomsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddNewItems(e.NewItems[0] as RoomShopUI);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveOldItems(e.OldItems[0] as RoomShopUI);
                break;
            case NotifyCollectionChangedAction.Reset:
                DeleteAllItems();
                break;
            default:
                break;
        }
    }

    private void DeleteAllItems()
    {
        foreach (RoomShopUI old_item in roomsUIContainer.transform.GetComponentsInChildren<RoomShopUI>())
        {
            Destroy(old_item.gameObject);
        }
    }

    private void RemoveOldItems(RoomShopUI old_item)
    {
        RoomShopUI[] room_inventorys = roomsUIContainer.transform.GetComponentsInChildren<RoomShopUI>();
        Destroy(room_inventorys.First(x => x.RoomInventoryUI == old_item.RoomInventoryUI).gameObject);
    }

    private void AddNewItems(RoomShopUI new_item)
    {
        _ = Instantiate(new_item, roomsUIContainer).GetComponent<RoomShopUI>();
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

