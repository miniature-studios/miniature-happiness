using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Level.Shop.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Transform roomsUIContainer;
        private Animator shopAnimator;

        public void OnShopRoomsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewItems(e.NewItems[0] as Room.View);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItems(e.OldItems[0] as Room.View);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DeleteAllItems();
                    break;
                default:
                    break;
            }
        }

        private void DeleteAllItems()
        {
            foreach (
                Room.View old_item in roomsUIContainer.transform.GetComponentsInChildren<Room.View>()
            )
            {
                Destroy(old_item.gameObject);
            }
        }

        private void RemoveOldItems(Room.View old_item)
        {
            Room.View[] room_inventorys =
                roomsUIContainer.transform.GetComponentsInChildren<Room.View>();
            Destroy(
                room_inventorys.First(x => x.RoomInventoryUI == old_item.RoomInventoryUI).gameObject
            );
        }

        private void AddNewItems(Room.View new_item)
        {
            _ = Instantiate(new_item, roomsUIContainer).GetComponent<Room.View>();
        }

        private void Awake()
        {
            shopAnimator = GetComponent<Animator>();
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
