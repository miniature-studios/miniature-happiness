using Level.Room;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Level.Inventory
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level.Inventory.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        public AssetLabelReference InventoryViewRef;

        [SerializeField]
        private Model model;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private TMP_Text buttonText;
        private Animator tilesInventoryAnimator;
        private bool inventoryShowed = false;

        private Dictionary<CoreModel, Room.View> modelViewMap = new();
        private List<Room.View> roomViews;

        private void Awake()
        {
            tilesInventoryAnimator = GetComponent<Animator>();
            foreach (
                GameObject prefab in Addressables
                    .LoadAssetsAsync<GameObject>(InventoryViewRef, null)
                    .WaitForCompletion()
            )
            {
                Debug.LogAssertion(prefab);
                Room.View view = prefab.GetComponent<Room.View>();
                if (view != null && view.CoreModel != null)
                {
                    modelViewMap.Add(view.CoreModel, view);
                }
            }
        }

        // Calls by button that open/closes inventory
        public void InventoryButtonClick()
        {
            inventoryShowed = !inventoryShowed;
            tilesInventoryAnimator.SetBool("Showed", inventoryShowed);
            buttonText.text = inventoryShowed ? "Close" : "Open";
        }

        public Room.View GetHoveredView()
        {
            return roomViews.FirstOrDefault(x => x.PointerOver);
        }

        public void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewItem(e.NewItems[0] as CoreModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItem(e.OldItems[0] as CoreModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RemoveAllItems();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    throw new ArgumentException();
            }
        }

        private void AddNewItem(CoreModel newItem)
        {
            Room.View view = roomViews.FirstOrDefault(x => x.CoreModel == newItem);
            if (view == null)
            {
                Room.View newRoomView = Instantiate(modelViewMap[newItem], container)
                    .GetComponent<Room.View>();
                newRoomView.Constructor(
                    () => newItem.Cost,
                    () => newItem.TariffProperties,
                    () => model.GetModelsCount(newRoomView.CoreModel),
                    () => newItem
                );
                newRoomView.enabled = true;
                roomViews.Add(newRoomView);
            }
        }

        private void RemoveOldItem(CoreModel oldItem)
        {
            Room.View existRoom = roomViews.Find(x => x.CoreModel == oldItem);
            if (existRoom.GetCount() == 0)
            {
                _ = roomViews.Remove(existRoom);
                Destroy(existRoom.gameObject);
            }
        }

        private void RemoveAllItems()
        {
            while (roomViews.Count > 0)
            {
                Room.View buffer = roomViews.Last();
                _ = roomViews.Remove(buffer);
                Destroy(buffer.gameObject);
            }
        }
    }
}
