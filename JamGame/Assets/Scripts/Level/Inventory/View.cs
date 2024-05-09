using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level/Inventory/Level.Inventory.View")]
    public class View : MonoBehaviour
    {
        //[Required]
        //[SerializeField]
        //private AssetLabelReference inventoryViewsLabel;

        [Required]
        [SerializeField]
        private Transform container;

        [Required]
        [SerializeField]
        private GameObject roomViewPrefab;

        //[Required]
        //[SerializeField]
        //private TMP_Text buttonText;

        [Required]
        [SerializeField]
        private Model model;

        //private Animator animator;
        //private bool isInventoryVisible = false;

        private Dictionary<InternalUid, Room.View> modelViewMap = new();

        //private List<Room.View> roomViews = new();

        private void Awake()
        {
            //animator = GetComponent<Animator>();

            model.InventoryRoomsCollectionChanged += OnInventoryChanged;
            // modelViewMap = AddressableTools<Room.View>.LoadAllFromLabel(inventoryViewsLabel);
        }

        // Called by button that open/closes inventory
        public void InventoryButtonClick()
        {
            // isInventoryVisible ^= true;
            //animator.SetBool("Showed", isInventoryVisible);
            //buttonText.text = isInventoryVisible ? "Close" : "Open";
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
            if (!modelViewMap.TryGetValue(room.Uid, out Room.View view))
            {
                GameObject viewGO = Instantiate(roomViewPrefab, container);
                view = viewGO.GetComponent<Room.View>();
                modelViewMap.Add(room.Uid, view);
            }

            view.AddCoreModel(room);
        }

        private void RemoveRoom(CoreModel room)
        {
            if (modelViewMap.TryGetValue(room.Uid, out Room.View view))
            {
                view.RemoveCoreModel(room);
                if (view.IsEmpty)
                {
                    _ = modelViewMap.Remove(room.Uid);
                    Destroy(view.gameObject);
                }
            }
            else
            {
                Debug.LogError("Cannot remove room from inventory: Uid is not present.");
            }
        }

        private void RemoveAllRooms()
        {
            //while (roomViews.Count > 0)
            //{
            //    Room.View roomView = roomViews.Last();
            //    RemoveRoomView(roomView);
            //}
        }

        public void ShowInventory()
        {
            //isInventoryVisible = true;
            //animator.SetBool("Showed", true);
            //buttonText.text = "Close";
        }
    }
}
