using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Level.Config;
using Level.Room;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Shop
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Transform roomsUIContainer;

        [SerializeField]
        private Transform employeesUIContainer;

        [SerializeField]
        private AssetLabelReference shopViewsLabel;
        private Dictionary<string, IResourceLocation> modelViewMap = new();

        [SerializeField]
        private EmployeeView employeeViewPrototype;

        private List<Room.View> roomsViewList = new();
        private List<EmployeeView> employeesViewList = new();

        private Animator animator;

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

        // Called by model when shop rooms collection changed.
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
            if (modelViewMap.TryGetValue(newRoom.Uid, out IResourceLocation location))
            {
                Room.View newRoomView = Instantiate(
                    AddressableTools<Room.View>.LoadAsset(location),
                    roomsUIContainer.transform
                );

                newRoomView.SetCoreModel(newRoom);
                newRoomView.enabled = true;
                roomsViewList.Add(newRoomView);
                newRoom.transform.SetParent(newRoomView.transform);
            }
            else
            {
                Debug.LogError($"Core model {newRoom.name} not presented in Shop View");
            }
        }

        private void RemoveOldRoom(CoreModel oldRoom)
        {
            Destroy(roomsViewList.Find(x => x.CoreModel == oldRoom).gameObject);
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

        // Called by model when shop employees collection changed.
        public void OnShopEmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewEmployee(e.NewItems[0] as EmployeeConfig);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldEmployee(e.OldItems[0] as EmployeeConfig);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DeleteAllEmployees();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    break;
            }
        }

        private void AddNewEmployee(EmployeeConfig newEmployee)
        {
            EmployeeView newEmployeeView = Instantiate(
                employeeViewPrototype,
                employeesUIContainer.transform
            );

            newEmployeeView.SetEmployeeConfig(newEmployee);
            newEmployeeView.enabled = true;
            employeesViewList.Add(newEmployeeView);
        }

        private void RemoveOldEmployee(EmployeeConfig oldEmployee)
        {
            Destroy(employeesViewList.Find(x => x.EmployeeConfig == oldEmployee).gameObject);
        }

        private void DeleteAllEmployees()
        {
            while (employeesViewList.Count > 0)
            {
                EmployeeView item = employeesViewList.Last();
                _ = employeesViewList.Remove(item);
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        // Called by button open shop.
        public void Open()
        {
            animator.SetBool("Showed", true);
        }

        // Called by button close shop.
        public void Close()
        {
            animator.SetBool("Showed", false);
        }
    }
}
