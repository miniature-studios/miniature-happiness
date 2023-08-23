using Common;
using Level.Room;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Inventory
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level.Inventory.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private AssetLabelReference inventoryViewRef;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private TMP_Text buttonText;
        private Animator tilesInventoryAnimator;
        private bool inventoryShowed = false;

        private Dictionary<string, IResourceLocation> modelViewMap = new();
        private List<Room.View> roomViews;

        private void Awake()
        {
            tilesInventoryAnimator = GetComponent<Animator>();
            foreach (
                LocationLinkPair<Room.View> pair in AddressablesTools.LoadAllFromLabel<Room.View>(
                    inventoryViewRef
                )
            )
            {
                modelViewMap.Add(pair.Link.CoreModel.HashCode, pair.ResourceLocation);
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
            if (modelViewMap.TryGetValue(newItem.HashCode, out IResourceLocation location))
            {
                Room.View newRoomView = Instantiate(
                    AddressablesTools.LoadAsset<Room.View>(location),
                    container
                );

                newRoomView.Constructor(() => newItem);
                roomViews.Add(newRoomView);
            }
            else
            {
                Debug.LogError($"Core model {newItem.name} not presented in Inventory View");
            }
        }

        private void RemoveOldItem(CoreModel oldItem)
        {
            Room.View existRoom = roomViews.Find(x => x.GetCoreModelInstance() == oldItem);
            _ = roomViews.Remove(existRoom);
            Destroy(existRoom.gameObject);
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
