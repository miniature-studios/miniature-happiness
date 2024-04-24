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
        [Pickle(typeof(Employee.CardView), LookupType = ObjectProviderType.Assets)]
        private Employee.CardView employeeCardPrefab;

        [Required]
        [SerializeField]
        private ShopContent content;

        [ReadOnly]
        [SerializeField]
        private List<Employee.CardView> employeeCards = new();

        [Required]
        [SerializeField]
        private Employee.DescriptionView cardInstance;

        private void Awake()
        {
            shopModel.EmployeeCollectionChanged += OnShopEmployeesChanged;
            ViewImpl mainView = GetComponentInParent<ViewImpl>(true);
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
            Employee.CardView newEmployeeCard = Instantiate(
                employeeCardPrefab,
                content.ContentTransform
            );
            newEmployeeCard.Initialize();
            newEmployeeCard.SetEmployeeConfig(newEmployee);
            employeeCards.Add(newEmployeeCard);

            newEmployeeCard.OnPointerEnterEvent += () => cardInstance.UpdateData(newEmployeeCard);
            newEmployeeCard.OnPointerExitEvent += () => cardInstance.UpdateData(null);
        }

        private void RemoveOldEmployee(EmployeeConfig oldEmployee)
        {
            Employee.CardView employeeCard = employeeCards.Find(x =>
                x.EmployeeConfig == oldEmployee
            );
            _ = employeeCards.Remove(employeeCard);
            Destroy(employeeCard.gameObject);
        }

        private void DeleteAllEmployees()
        {
            while (employeeCards.Count > 0)
            {
                Employee.CardView employeeCard = employeeCards.Last();
                _ = employeeCards.Remove(employeeCard);
                Destroy(employeeCard.gameObject);
            }
        }
    }
}
