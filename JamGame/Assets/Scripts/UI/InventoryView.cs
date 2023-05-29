using Common;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventoryView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform container;
    [SerializeField] private TMP_Text button_text;
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

    public void OnInventoryChanged(List<RoomInventoryUI> items, NotifyCollectionChangedAction action)
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

    private void ReplaceAllItems(List<RoomInventoryUI> NewItems)
    {
        foreach (RoomInventoryUI old_item in container.transform.GetComponentsInChildren<RoomInventoryUI>())
        {
            old_item.Counter = 0;
        }
        AddNewItems(NewItems);
    }

    private void RemoveOldItems(List<RoomInventoryUI> old_items)
    {
        RoomInventoryUI[] room_inventorys = container.transform.GetComponentsInChildren<RoomInventoryUI>();
        foreach (RoomInventoryUI old_item in old_items)
        {
            room_inventorys.First(x => x.TileUnion == old_item.TileUnion).Counter--;
        }
    }

    private void AddNewItems(List<RoomInventoryUI> new_items)
    {
        RoomInventoryUI[] room_inventorys = container.transform.GetComponentsInChildren<RoomInventoryUI>();
        for (int i = 0; i < new_items.Count; i++)
        {
            RoomInventoryUI existed = room_inventorys.FirstOrDefault(x => x.TileUnion == new_items[i].TileUnion);
            if (existed != null)
            {
                existed.Counter++;
            }
            else
            {
                new_items[i] = Instantiate(new_items[i], container).GetComponent<RoomInventoryUI>();
            }
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

