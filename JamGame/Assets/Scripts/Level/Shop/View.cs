using Common;
using Level.Room;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        private Dictionary<CoreModel, Room.View> modelViewMap = new();
        private List<Room.View> viewList = new();

        private void Awake()
        {
            shopAnimator = GetComponent<Animator>();
            foreach (GameObject prefab in PrefabsTools.GetAllAssetsPrefabs())
            {
                Room.View view = prefab.GetComponent<Room.View>();
                if (view != null && view.CoreModel != null)
                {
                    modelViewMap.Add(view.CoreModel, view);
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
            Room.View newRoomView = Instantiate(modelViewMap[newItem], roomsUIContainer)
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
