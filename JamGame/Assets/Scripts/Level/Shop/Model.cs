using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Level.Config;
using Level.Room;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.Model")]
    public class Model : MonoBehaviour
    {
        private ObservableCollection<CoreModel> roomsInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> RoomsCollectionChanged = new();

        private ObservableCollection<EmployeeConfig> employeesInShop = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> EmployeeCollectionChanged =
            new();

        private void Awake()
        {
            roomsInShop.CollectionChanged += RoomsCollectionChanged.Invoke;
            employeesInShop.CollectionChanged += EmployeeCollectionChanged.Invoke;
        }

        public void ResetRooms(IEnumerable<CoreModel> rooms)
        {
            ClearRooms();
            foreach (CoreModel room in rooms)
            {
                AddRoom(room);
            }
        }

        public void AddRoom(CoreModel room)
        {
            roomsInShop.Add(room);
        }

        public CoreModel BorrowRoom(CoreModel room)
        {
            return roomsInShop.Remove(room) ? room : null;
        }

        public void ClearRooms()
        {
            foreach (CoreModel room in roomsInShop)
            {
                Destroy(room.gameObject);
            }
            roomsInShop.Clear();
        }

        public void ResetEmployees(IEnumerable<EmployeeConfig> employees)
        {
            ClearEmployees();
            foreach (EmployeeConfig employee in employees)
            {
                AddEmployee(employee);
            }
        }

        public void AddEmployee(EmployeeConfig employee)
        {
            employeesInShop.Add(employee);
        }

        public EmployeeConfig BorrowEmployee(EmployeeConfig employee)
        {
            return employeesInShop.Remove(employee) ? employee : null;
        }

        public void ClearEmployees()
        {
            employeesInShop.Clear();
        }
    }
}
