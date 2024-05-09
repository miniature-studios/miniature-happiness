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
        [Required]
        [SerializeField]
        private Transform container;

        [Required]
        [SerializeField]
        private GameObject roomViewPrefab;

        [Required]
        [SerializeField]
        private Model model;

        [Required]
        [SerializeField]
        private RoomDescriptionView description;

        private Dictionary<InternalUid, Room.View> modelViewMap = new();

        private void Awake()
        {
            model.InventoryRoomsCollectionChanged += OnInventoryChanged;
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

                view.OnHoverChanged += (sender, hovered) =>
                    description.OnActiveRoomChanged(hovered ? sender : null);

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
            foreach (Room.View view in modelViewMap.Values)
            {
                view.RemoveAllCoreModels();
                Destroy(view.gameObject);
            }

            modelViewMap.Clear();
        }
    }
}
