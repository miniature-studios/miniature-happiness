using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Inventory
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level/Inventory/Level.Inventory.View")]
    public class View : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private AssetLabelReference inventoryViewsLabel;

        [Required]
        [SerializeField]
        private Transform container;

        [Required]
        [SerializeField]
        private TMP_Text buttonText;

        [Required]
        [SerializeField]
        private Model model;

        private Animator animator;
        private bool isInventoryVisible = false;

        private Dictionary<InternalUid, IResourceLocation> modelViewMap = new();
        private List<Room.View> roomViews = new();

        private void Awake()
        {
            animator = GetComponent<Animator>();

            model.InventoryRoomsCollectionChanged += OnInventoryChanged;
            foreach (
                AssetWithLocation<Room.View> invView in AddressableTools<Room.View>.LoadAllFromLabel(
                    inventoryViewsLabel
                )
            )
            {
                modelViewMap.Add(invView.Asset.Uid, invView.Location);
            }
        }

        // Called by button that open/closes inventory
        public void InventoryButtonClick()
        {
            isInventoryVisible ^= true;
            animator.SetBool("Showed", isInventoryVisible);
            buttonText.text = isInventoryVisible ? "Close" : "Open";
        }

        public void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddRoom(e.NewItems[0] as CoreModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveRoom(e.OldItems[0] as CoreModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RemoveAllRooms();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    throw new ArgumentException();
            }
        }

        private void AddRoom(CoreModel room)
        {
            Room.View foundView = roomViews.Find(x => x.Uid == room.Uid);
            if (foundView != null)
            {
                foundView.AddCoreModel(room);
            }
            else if (modelViewMap.TryGetValue(room.Uid, out IResourceLocation location))
            {
                Room.View newRoomView = Instantiate(
                    AddressableTools<Room.View>.LoadAsset(location),
                    container
                );

                newRoomView.AddCoreModel(room);
                roomViews.Add(newRoomView);
            }
            else
            {
                Debug.LogError($"Core model {room.name} not presented in Inventory View");
            }
        }

        private void RemoveRoom(CoreModel room)
        {
            Room.View roomView = roomViews.Find(x => x.Uid == room.Uid);
            roomView.RemoveCoreModel(room);
            if (roomView.IsEmpty)
            {
                RemoveRoomView(roomView);
            }
        }

        private void RemoveAllRooms()
        {
            while (roomViews.Count > 0)
            {
                Room.View roomView = roomViews.Last();
                RemoveRoomView(roomView);
            }
        }

        public void ShowInventory()
        {
            isInventoryVisible = true;
            animator.SetBool("Showed", true);
            buttonText.text = "Close";
        }

        private void RemoveRoomView(Room.View roomView)
        {
            _ = roomViews.Remove(roomView);
            Destroy(roomView.gameObject);
        }
    }
}
