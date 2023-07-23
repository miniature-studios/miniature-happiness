using System;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Level.Inventory
{
    [AddComponentMenu("Level.Inventory.View")]
    public class View : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
                    AddNewItem(e.NewItems[0] as Room.Model);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItem(e.OldItems[0] as Room.Model);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DeleteAllItems();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    throw new ArgumentException();
            }
        }

        private void DeleteAllItems()
        {
            foreach (Room.Model old_item in container.transform.GetComponentsInChildren<Room.Model>())
            {
                old_item.Count = 0;
            }
        }

        private void RemoveOldItem(Room.Model old_item)
        {
            Room.Model[] room_models = container.transform.GetComponentsInChildren<Room.Model>();
            room_models.First(x => x.TileUnion == old_item.TileUnion).Count--;
        }

        private void AddNewItem(Room.Model new_item)
        {
            Room.Model[] room_inventorys = container.transform.GetComponentsInChildren<Room.Model>();
            Room.Model existed = room_inventorys.FirstOrDefault(
                x => x.TileUnion == new_item.TileUnion
            );
            if (existed != null)
            {
                existed.Count++;
            }
            else
            {
                Room.View new_room_view = Instantiate(new_item, container).GetComponent<Room.View>();
                new_room_view.enabled = true;
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
}
