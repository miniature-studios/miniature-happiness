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
        private TMP_Text nameText;

        private Func<EmployeeConfig, Result> employeeBuying;

        private void Awake()
        {
            employeeBuying = GetComponentInParent<Controller>().TryBuyEmployee;
        }

        private void Update()
        {
            nameText.text = $"Name: {employeeConfig.Name}";
        }

        public void SetEmployeeConfig(EmployeeConfig employeeConfig)
        {
            this.employeeConfig = employeeConfig;
        }

        // Called be pressing button
        public void TryHireEmployee()
        {
            if (employeeBuying(EmployeeConfig).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
