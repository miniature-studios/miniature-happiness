using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.Rooms")]
    internal class Rooms : MonoBehaviour, IShopContent
    {
        [Required]
        [SerializeField]
        private Model shopModel;

        [Required]
        [SerializeField]
        private ShopContent content;

        [Required]
        [SerializeField]
        private Tab tab;

        [SerializeField]
        private AssetCollectionLoader<Room.Plank> roomPlanksLoader = new();

        [SerializeField]
        private AssetCollectionLoader<Room.Card> roomCardLoader = new();

        [ReadOnly]
        [SerializeField]
        private List<Room.Plank> roomPlanks = new();

        public event Action OnSwitchedTo;

        private void Awake()
        {
            shopModel.RoomsCollectionChanged += OnShopRoomsChanged;
            roomPlanksLoader.PrepareCollection();
            roomCardLoader.PrepareCollection();
        }

        private void OnShopRoomsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewRoom(e.NewItems[0] as CoreModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldRoom(e.OldItems[0] as CoreModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DeleteAllRooms();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    break;
            }
        }

        private void AddNewRoom(CoreModel newRoom)
        {
            Room.Plank foundRoomPlank = roomPlanks.Find(x => x.Uid == newRoom.Uid);
            if (foundRoomPlank != null)
            {
                foundRoomPlank.AddCoreModel(newRoom);
                return;
            }

            if (
                !roomPlanksLoader.Collection.TryGetValue(
                    newRoom.Uid,
                    out IResourceLocation plankLocation
                )
            )
            {
                Debug.LogError($"Core model {newRoom.name} not presented in Shop View");
                return;
            }

            Room.Plank newRoomPlank = Instantiate(
                AddressableTools<Room.Plank>.LoadAsset(plankLocation),
                content.ContentTransform
            );

            if (
                !roomCardLoader.Collection.TryGetValue(
                    newRoom.Uid,
                    out IResourceLocation cardLocation
                )
            )
            {
                Debug.LogError($"Core model {newRoom.name} not presented in Shop View");
                return;
            }

            Room.Card cardPrefab = AddressableTools<Room.Card>.LoadAsset(cardLocation);
            newRoomPlank.AddCard(cardPrefab);
            newRoomPlank.AddCoreModel(newRoom);
            newRoomPlank.enabled = true;
            roomPlanks.Add(newRoomPlank);
        }

        private void RemoveOldRoom(CoreModel oldRoom)
        {
            Room.Plank roomView = roomPlanks.Find(x => x.Uid == oldRoom.Uid);
            roomView.RemoveCoreModel(oldRoom);
            if (roomView.IsEmpty)
            {
                RemoveRoomView(roomView);
            }
        }

        private void DeleteAllRooms()
        {
            while (roomPlanks.Count > 0)
            {
                Room.Plank roomView = roomPlanks.Last();
                RemoveRoomView(roomView);
            }
        }

        private void RemoveRoomView(Room.Plank roomView)
        {
            _ = roomPlanks.Remove(roomView);
            Destroy(roomView.gameObject);
        }

        [Button]
        public void Show()
        {
            content.Show();
            tab.Activate();
            OnSwitchedTo.Invoke();
        }

        [Button]
        public void Hide()
        {
            content.Hide();
            tab.Deactivate();
        }
    }
}
