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
                    AddNewItems(e.NewItems[0] as Room.Model);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItems(e.OldItems[0] as Room.Model);
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
                Room.Model old_item in roomsUIContainer.transform.GetComponentsInChildren<Room.Model>()
            )
            {
                Destroy(old_item.gameObject);
            }
        }

        private void RemoveOldItems(Room.Model old_item)
        {
            Room.Model[] room_models =
                roomsUIContainer.transform.GetComponentsInChildren<Room.Model>();
            Destroy(
                room_models.First(x => x == old_item).gameObject
            );
        }

        private void AddNewItems(Room.Model new_item)
        {
            Room.View new_room_view = Instantiate(new_item, roomsUIContainer).GetComponent<Room.View>();
            new_room_view.enabled = true;
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
