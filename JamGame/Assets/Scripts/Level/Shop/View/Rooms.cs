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
        [Pickle(typeof(Room.CardView), LookupType = ObjectProviderType.Assets)]
        private Room.CardView roomCardPrefab;

        [ReadOnly]
        [SerializeField]
        private List<Room.CardView> roomCards = new();

        [Required]
        [SerializeField]
        private Room.DescriptionView descriptionViewInstance;

        private void Awake()
        {
            shopModel.RoomsCollectionChanged += OnShopRoomsChanged;
            ViewImpl mainView = GetComponentInParent<ViewImpl>(true);
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
            Room.CardView foundRoomCard = roomCards.Find(x => x.Uid == newRoom.Uid);
            if (foundRoomCard != null)
            {
                foundRoomCard.AddCoreModel(newRoom);
                return;
            }

            Room.CardView newRoomCard = Instantiate(roomCardPrefab, content.ContentTransform);
            newRoomCard.Initialize();
            newRoomCard.AddCoreModel(newRoom);
            newRoomCard.OnPointerEnterEvent += () =>
                descriptionViewInstance.UpdateData(newRoomCard);
            newRoomCard.OnPointerExitEvent += () => descriptionViewInstance.UpdateData(null);
        }

        private void RemoveOldRoom(CoreModel oldRoom)
        {
            Room.CardView roomCard = roomCards.Find(x => x.Uid == oldRoom.Uid);
            roomCard.RemoveCoreModel(oldRoom);
            if (roomCard.IsEmpty)
            {
                RemoveRoom(roomCard);
            }
        }

        private void DeleteAllRooms()
        {
            while (roomCards.Count > 0)
            {
                Room.CardView roomCard = roomCards.Last();
                RemoveRoom(roomCard);
            }
        }

        private void RemoveRoom(Room.CardView roomCard)
        {
            _ = roomCards.Remove(roomCard);
            Destroy(roomCard.gameObject);
        }
    }
}
