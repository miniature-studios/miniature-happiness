using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.Rooms")]
    internal class Rooms : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Model shopModel;

        [Required]
        [SerializeField]
        private ShopContent content;

        [Required]
        [SerializeField]
        [Pickle(typeof(Room.Plank), LookupType = ObjectProviderType.Assets)]
        private Room.Plank roomPlankPrefab;

        [Required]
        [SerializeField]
        [Pickle(typeof(Room.Card), LookupType = ObjectProviderType.Assets)]
        private Room.Card roomCardPrefab;

        [ReadOnly]
        [SerializeField]
        private List<Room.Plank> roomPlanks = new();

        [ReadOnly]
        [SerializeField]
        private Room.Card cardInstance;

        private void Awake()
        {
            shopModel.RoomsCollectionChanged += OnShopRoomsChanged;
            ViewImpl mainView = GetComponentInParent<ViewImpl>(true);
            cardInstance = Instantiate(roomCardPrefab, mainView.CardParent);
            cardInstance.Hide();
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

            Room.Plank newRoomPlank = Instantiate(roomPlankPrefab, content.ContentTransform);
            newRoomPlank.Initialize();
            newRoomPlank.AddCoreModel(newRoom);
            newRoomPlank.OnPointerEnterEvent += () =>
            {
                cardInstance.UpdateData(newRoomPlank);
                cardInstance.Show();
            };
            newRoomPlank.OnPointerExitEvent += cardInstance.Hide;
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
    }
}
