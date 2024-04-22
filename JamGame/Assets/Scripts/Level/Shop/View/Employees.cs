using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Level.Config;
using Level.Shop.Employee;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.Employees")]
    internal class Employees : MonoBehaviour, IShopContent
    {
        [Required]
        [SerializeField]
        private Model shopModel;

        [Required]
        [SerializeField]
        [Pickle(typeof(Plank), LookupType = ObjectProviderType.Assets)]
        private Plank employeePlankPrefab;

        [Required]
        [SerializeField]
        private ShopContent content;

        [Required]
        [SerializeField]
        private Tab tab;

        [ReadOnly]
        [SerializeField]
        private List<Plank> employeeViews = new();

        public event Action OnSwitchedTo;

        private void Awake()
        {
            shopModel.EmployeeCollectionChanged += OnShopEmployeesChanged;
        }

        private void OnShopEmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            Plank newEmployeePlank = Instantiate(employeePlankPrefab, content.ContentTransform);
            newEmployeePlank.Initialize();
            newEmployeePlank.SetEmployeeConfig(newEmployee);
            employeeViews.Add(newEmployeePlank);
        }

        private void RemoveOldEmployee(EmployeeConfig oldEmployee)
        {
            Plank employee = employeeViews.Find(x => x.EmployeeConfig == oldEmployee);
            _ = employeeViews.Remove(employee);
            Destroy(employee.gameObject);
        }

        private void DeleteAllEmployees()
        {
            while (employeeViews.Count > 0)
            {
                Plank item = employeeViews.Last();
                _ = employeeViews.Remove(item);
                Destroy(item.gameObject);
            }
        }

        [Button]
        public void Show()
        {
            content.Show();
            tab.Activate();
            OnSwitchedTo.Invoke();
        }

        [Button]
        public void Hide()
        {
            content.Hide();
            tab.Deactivate();
        }
    }
}
