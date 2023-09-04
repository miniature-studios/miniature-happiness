using Common;
using Level.Config;
using System;
using TMPro;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level.Shop.EmployeeView")]
    public class EmployeeView : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private EmployeeConfig employeeConfig;
        public EmployeeConfig EmployeeConfig => employeeConfig;

        [SerializeField]
        private TMP_Text nameLabel;

        [SerializeField]
        private TMP_Text hireCostLabel;

        [SerializeField]
        private TMP_Text professionLabel;

        [SerializeField]
        private TMP_Text quirkLabel;

        private Func<EmployeeConfig, Result> employeeBuying;

        private void Awake()
        {
            employeeBuying = GetComponentInParent<Controller>().TryBuyEmployee;
        }

        private void Update()
        {
            nameLabel.text = $"{employeeConfig.Name}";
            hireCostLabel.text = $"Hire cost: {employeeConfig.HireCost}$";
            professionLabel.text = $"{employeeConfig.Profession}";
            quirkLabel.text = $"{employeeConfig.Quirk}";
        }

        public void SetEmployeeConfig(EmployeeConfig employeeConfig)
        {
            this.employeeConfig = employeeConfig;
        }

        // Called be pressing button.
        public void TryHireEmployee()
        {
            if (employeeBuying(EmployeeConfig).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
