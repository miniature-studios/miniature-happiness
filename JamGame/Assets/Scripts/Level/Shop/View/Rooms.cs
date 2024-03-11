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
    public partial class ViewImpl
    {
        [SerializeField]
        private Transform roomsUIContainer;

        [SerializeField]
        private AssetLabelReference shopViewsLabel;
        private Dictionary<InternalUid, IResourceLocation> modelViewMap = new();

        [ReadOnly]
        [SerializeField]
        private List<Room.View> roomsViewList = new();

        private void InitModelViewMap()
        {
            IEnumerable<AssetWithLocation<Room.View>> shopViewLocations =
                AddressableTools<Room.View>.LoadAllFromLabel(shopViewsLabel);

            foreach (AssetWithLocation<Room.View> shopView in shopViewLocations)
            {
                modelViewMap.Add(shopView.Asset.Uid, shopView.Location);
            }
        }

        public void OnShopRoomsChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            Room.View foundView = roomsViewList.Find(x => x.CoreModel.CompareUid(newRoom));
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
                roomsViewList.Add(newRoomView);
            }
            else
            {
                Debug.LogError($"Core model {newRoom.name} not presented in Shop View");
            }
        }

        private void RemoveOldRoom(CoreModel oldRoom)
        {
            Room.View room = roomsViewList.Find(x => x.CoreModel.CompareUid(oldRoom));
            room.RemoveCoreModel(oldRoom);
        }

        private void DeleteAllRooms()
        {
            while (roomsViewList.Count > 0)
            {
                Room.View item = roomsViewList.Last();
                _ = roomsViewList.Remove(item);
                Destroy(item.gameObject);
            }
        }
    }
}
