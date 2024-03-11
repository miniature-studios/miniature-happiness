using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Level.Config;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.Employees")]
    internal class Employees : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Model shopModel;

        [Required]
        [SerializeField]
        [Pickle(typeof(EmployeeView), LookupType = ObjectProviderType.Assets)]
        private EmployeeView employeeViewPrototype;

        [Required]
        [SerializeField]
        private Transform employeesUIContainer;

        [ReadOnly]
        [SerializeField]
        private List<EmployeeView> employeeViews = new();

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
            EmployeeView newEmployeeView = Instantiate(
                employeeViewPrototype,
                employeesUIContainer.transform
            );

            newEmployeeView.SetEmployeeConfig(newEmployee);
            newEmployeeView.enabled = true;
            employeeViews.Add(newEmployeeView);
        }

        private void RemoveOldEmployee(EmployeeConfig oldEmployee)
        {
            EmployeeView employee = employeeViews.Find(x => x.EmployeeConfig == oldEmployee);
            _ = employeeViews.Remove(employee);
            Destroy(employee.gameObject);
        }

        private void DeleteAllEmployees()
        {
            while (employeeViews.Count > 0)
            {
                EmployeeView item = employeeViews.Last();
                _ = employeeViews.Remove(item);
                Destroy(item.gameObject);
            }
        }
    }
}
