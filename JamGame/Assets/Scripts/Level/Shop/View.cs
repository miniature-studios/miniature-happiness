using Common;
using Level.Room;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Level.Shop
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level.Shop.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Transform roomsUIContainer;
        private Animator shopAnimator;
        private Dictionary<UniqueId, Room.View> modelViewMap;
        private List<Room.View> viewList = new();

        private void Awake()
        {
            shopAnimator = GetComponent<Animator>();
            foreach (GameObject prefab in PrefabsTools.GetAllAssetsPrefabs())
            {
                if (prefab.TryGetComponent(out Room.View view))
                {
                    modelViewMap.Add(view.UniqueId, view);
                }
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
            Room.View newRoomView = Instantiate(modelViewMap[newItem.UniqueId], roomsUIContainer)
                .GetComponent<Room.View>();
            newRoomView.Constructor(
                () => newItem.Cost,
                () => newItem.TariffProperties,
                () => newItem
            );
            newRoomView.enabled = true;
            viewList.Add(newRoomView);
        }

        private void RemoveOldItem(CoreModel oldItem)
        {
            Destroy(viewList.Find(x => x.GetCoreModelInstance() == oldItem).gameObject);
        }

        private void DeleteAllItems()
        {
            foreach (Room.View views in viewList)
            {
                Destroy(views.gameObject);
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
