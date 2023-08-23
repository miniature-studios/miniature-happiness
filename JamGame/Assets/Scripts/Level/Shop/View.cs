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
        private AssetLabelReference shopViewReference;
        private Animator shopAnimator;
        private Dictionary<string, IResourceLocation> modelViewMap = new();
        private List<Room.View> viewList = new();

        private void Awake()
        {
            shopAnimator = GetComponent<Animator>();
            foreach (
                LocationLinkPair<Room.View> pair in AddressablesTools.LoadAllFromLabel<Room.View>(
                    shopViewReference
                )
            )
            {
                modelViewMap.Add(pair.Link.CoreModel.HashCode, pair.ResourceLocation);
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
            if (modelViewMap.TryGetValue(newItem.HashCode, out IResourceLocation location))
            {
                Room.View newRoomView = Instantiate(
                    AddressablesTools.LoadAsset<Room.View>(location),
                    roomsUIContainer.transform
                );

                newRoomView.Constructor(() => newItem);
                newRoomView.enabled = true;
                viewList.Add(newRoomView);
            }
            else
            {
                Debug.LogError($"Core model {newItem.name} not presented in Shop View");
            }
        }

        private void RemoveOldItem(CoreModel oldItem)
        {
            Destroy(viewList.Find(x => x.GetCoreModelInstance() == oldItem).gameObject);
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
            shopAnimator.SetBool("Showed", true);
        }

        public void Close()
        {
            shopAnimator.SetBool("Showed", false);
        }
    }
}
