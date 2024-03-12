using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Level.Config;
using Level.Room;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.Model")]
    public class Model : MonoBehaviour
    {
        private ObservableCollection<CoreModel> roomsInShop = new();
        public event NotifyCollectionChangedEventHandler RoomsCollectionChanged
        {
            add => roomsInShop.CollectionChanged += value;
            remove => roomsInShop.CollectionChanged -= value;
        }

        private ObservableCollection<EmployeeConfig> employeesInShop = new();
        public event NotifyCollectionChangedEventHandler EmployeeCollectionChanged
        {
            add => employeesInShop.CollectionChanged += value;
            remove => employeesInShop.CollectionChanged -= value;
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

        public Result<CoreModel> BorrowRoom(InternalUid roomUid)
        {
            CoreModel foundRoom = roomsInShop.FirstOrDefault(x => x.Uid == roomUid);
            if (foundRoom != null)
            {
                _ = roomsInShop.Remove(foundRoom);
                return new SuccessResult<CoreModel>(foundRoom);
            }
            else
            {
                return new FailResult<CoreModel>("No any CoreModel with this Uid.");
            }
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
