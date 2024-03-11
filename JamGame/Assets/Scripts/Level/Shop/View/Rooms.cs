using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

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
        private Transform roomsUIContainer;

        [Required]
        [SerializeField]
        private AssetLabelReference shopViewsLabel;
        private Dictionary<InternalUid, IResourceLocation> modelViewMap = new();

        [ReadOnly]
        [SerializeField]
        private List<Room.View> roomViews = new();

        private void Awake()
        {
            shopModel.RoomsCollectionChanged += OnShopRoomsChanged;
            InitModelViewMap();
        }

        private void InitModelViewMap()
        {
            IEnumerable<AssetWithLocation<Room.View>> shopViewLocations =
                AddressableTools<Room.View>.LoadAllFromLabel(shopViewsLabel);

            foreach (AssetWithLocation<Room.View> shopView in shopViewLocations)
            {
                modelViewMap.Add(shopView.Asset.Uid, shopView.Location);
            }
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
            Room.View foundView = roomViews.Find(x => x.CoreModel.UidEquals(newRoom));
            if (foundView != null)
            {
                foundView.AddCoreModel(newRoom);
            }
            else if (modelViewMap.TryGetValue(newRoom.Uid, out IResourceLocation location))
            {
                Room.View newRoomView = Instantiate(
                    AddressableTools<Room.View>.LoadAsset(location),
                    roomsUIContainer.transform
                );

                newRoomView.AddCoreModel(newRoom);
                newRoomView.enabled = true;
                roomViews.Add(newRoomView);
            }
            else
            {
                Debug.LogError($"Core model {newRoom.name} not presented in Shop View");
            }
        }

        private void RemoveOldRoom(CoreModel oldRoom)
        {
            Room.View roomView = roomViews.Find(x => x.CoreModel.UidEquals(oldRoom));
            roomView.RemoveCoreModel(oldRoom);
            if (roomView.IsEmpty)
            {
                RemoveRoomView(roomView);
            }
        }

        private void DeleteAllRooms()
        {
            while (roomViews.Count > 0)
            {
                Room.View roomView = roomViews.Last();
                RemoveRoomView(roomView);
            }
        }

        private void RemoveRoomView(Room.View roomView)
        {
            _ = roomViews.Remove(roomView);
            Destroy(roomView.gameObject);
        }
    }
}
