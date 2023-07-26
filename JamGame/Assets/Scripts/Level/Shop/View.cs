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
                    AddNewItems(e.NewItems[0] as Room.MenuView);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItems(e.OldItems[0] as Room.MenuView);
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
                Room.MenuView old_item in roomsUIContainer.transform.GetComponentsInChildren<Room.MenuView>()
            )
            {
                Destroy(old_item.gameObject);
            }
        }

        private void RemoveOldItems(Room.MenuView old_item)
        {
            Room.MenuView[] room_inventorys =
                roomsUIContainer.transform.GetComponentsInChildren<Room.MenuView>();
            Destroy(
                room_inventorys.First(x => x.RoomInventoryUI == old_item.RoomInventoryUI).gameObject
            );
        }

        private void AddNewItems(Room.MenuView new_item)
        {
            _ = Instantiate(new_item, roomsUIContainer).GetComponent<Room.MenuView>();
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
