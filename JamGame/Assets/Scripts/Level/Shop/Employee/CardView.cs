using System;
using Level.Config;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop.Employee/Level.Shop.Employee.CardView")]
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [ReadOnly]
        [SerializeField]
        private EmployeeConfig employeeConfig;
        public EmployeeConfig EmployeeConfig => employeeConfig;

        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        [Required]
        [SerializeField]
        private TMP_Text hireCostLabel;

        [Required]
        [SerializeField]
        private TMP_Text professionLabel;

        private Controller controller;

        public event Action OnPointerEnterEvent;
        public event Action OnPointerExitEvent;

        public void Initialize()
        {
            controller = GetComponentInParent<Controller>(true);
        }

        public void SetEmployeeConfig(EmployeeConfig employeeConfig)
        {
            this.employeeConfig = employeeConfig;
            UpdateData();
        }

        private void UpdateData()
        {
            nameLabel.text = EmployeeConfig.Name;
            hireCostLabel.text = EmployeeConfig.HireCost.ToString();
            professionLabel.text = EmployeeConfig.Profession;
        }

        // Called be pressing button.
        public void TryHireEmployee()
        {
            if (controller.TryBuyEmployee(EmployeeConfig).Success)
            {
                Destroy(gameObject);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke();
        }
    }
}
