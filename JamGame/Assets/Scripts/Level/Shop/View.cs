using Common;
using Level.Room;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Shop
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level.Shop.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Transform roomsUIContainer;

        [SerializeField]
        private AssetLabelReference shopViewsLabel;
        private Animator animator;
        private Dictionary<string, IResourceLocation> modelViewMap = new();
        private List<Room.View> viewList = new();

        private void Awake()
        {
            animator = GetComponent<Animator>();
            foreach (
                AssetWithLocation<Room.View> shopView in AddressableTools<Room.View>.LoadAllFromLabel(
                    shopViewsLabel
                )
            )
            {
                modelViewMap.Add(shopView.Asset.Uid, shopView.Location);
            }
        }

        public void OnShopRoomsChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    DeleteAllItems();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    break;
            }
        }

        private void AddNewItem(CoreModel newItem)
        {
            if (modelViewMap.TryGetValue(newItem.Uid, out IResourceLocation location))
            {
                Room.View newRoomView = Instantiate(
                    AddressableTools<Room.View>.LoadAsset(location),
                    roomsUIContainer.transform
                );

                newRoomView.SetCoreModel(newItem);
                newRoomView.enabled = true;
                viewList.Add(newRoomView);
                newItem.transform.SetParent(newRoomView.transform);
            }
            else
            {
                Debug.LogError($"Core model {newItem.name} not presented in Shop View");
            }
        }

        private void RemoveOldItem(CoreModel oldItem)
        {
            Destroy(viewList.Find(x => x.CoreModel == oldItem).gameObject);
        }

        private void DeleteAllItems()
        {
            while (viewList.Count > 0)
            {
                Room.View item = viewList.Last();
                _ = viewList.Remove(item);
                Destroy(item.gameObject);
            }
        }

        public void Open()
        {
            animator.SetBool("Showed", true);
        }

        public void Close()
        {
            animator.SetBool("Showed", false);
        }
    }
}
