using System;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventoryView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Transform container;

    [SerializeField]
    private TMP_Text button_text;
    private Animator tilesInventoryAnimator;
    public UnityEvent<bool> PointerOverEvent;

    private void Awake()
    {
        tilesInventoryAnimator = GetComponent<Animator>();
    }

    private bool inventoryShowed = false;

    public void InventoryButtonClick()
    {
        inventoryShowed = !inventoryShowed;
        tilesInventoryAnimator.SetBool("Showed", inventoryShowed);
        button_text.text = inventoryShowed ? "Close" : "Open";
    }

    public void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddNewItem(e.NewItems[0] as RoomInventoryUI);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveOldItem(e.OldItems[0] as RoomInventoryUI);
                break;
            case NotifyCollectionChangedAction.Reset:
                DeleteAllItems();
                break;
            default:
                Debug.LogError($"Unexpected variant of NotifyCollectionChangedAction: {e.Action}");
                throw new ArgumentException();
        }
    }

    private void DeleteAllItems()
    {
        foreach (
            RoomInventoryUI old_item in container.transform.GetComponentsInChildren<RoomInventoryUI>()
        )
        {
            old_item.Counter = 0;
        }
    }

    private void RemoveOldItem(RoomInventoryUI old_item)
    {
        RoomInventoryUI[] room_inventorys =
            container.transform.GetComponentsInChildren<RoomInventoryUI>();
        room_inventorys.First(x => x.TileUnion == old_item.TileUnion).Counter--;
    }

    private void AddNewItem(RoomInventoryUI new_item)
    {
        RoomInventoryUI[] room_inventorys =
            container.transform.GetComponentsInChildren<RoomInventoryUI>();
        RoomInventoryUI existed = room_inventorys.FirstOrDefault(
            x => x.TileUnion == new_item.TileUnion
        );
        if (existed != null)
        {
            existed.Counter++;
        }
        else
        {
            _ = Instantiate(new_item, container).GetComponent<RoomInventoryUI>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerOverEvent?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerOverEvent?.Invoke(false);
    }
}
