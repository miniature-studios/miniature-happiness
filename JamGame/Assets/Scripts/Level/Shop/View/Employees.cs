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
        [Pickle(typeof(Employee.Plank), LookupType = ObjectProviderType.Assets)]
        private Employee.Plank employeePlankPrefab;

        [Required]
        [SerializeField]
        [Pickle(typeof(Employee.Card), LookupType = ObjectProviderType.Assets)]
        private Employee.Card employeeCardPrefab;

        [Required]
        [SerializeField]
        private ShopContent content;

        [ReadOnly]
        [SerializeField]
        private List<Employee.Plank> employeePlanks = new();

        [ReadOnly]
        [SerializeField]
        private Employee.Card cardInstance;

        private void Awake()
        {
            shopModel.EmployeeCollectionChanged += OnShopEmployeesChanged;
            ViewImpl mainView = GetComponentInParent<ViewImpl>(true);
            cardInstance = Instantiate(employeeCardPrefab, mainView.CardParent);
            cardInstance.Hide();
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
            Employee.Plank newEmployeePlank = Instantiate(
                employeePlankPrefab,
                content.ContentTransform
            );
            newEmployeePlank.Initialize();
            newEmployeePlank.SetEmployeeConfig(newEmployee);
            employeePlanks.Add(newEmployeePlank);

            newEmployeePlank.OnPointerEnterEvent += () =>
            {
                cardInstance.UpdateData(newEmployeePlank);
                cardInstance.Show();
            };
            newEmployeePlank.OnPointerExitEvent += cardInstance.Hide;
        }

        private void RemoveOldEmployee(EmployeeConfig oldEmployee)
        {
            Employee.Plank employee = employeePlanks.Find(x => x.EmployeeConfig == oldEmployee);
            _ = employeePlanks.Remove(employee);
            Destroy(employee.gameObject);
        }

        private void DeleteAllEmployees()
        {
            while (employeePlanks.Count > 0)
            {
                Employee.Plank item = employeePlanks.Last();
                _ = employeePlanks.Remove(item);
                Destroy(item.gameObject);
            }
        }
    }
}
