using Level.Config;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop.Employee/Level.Shop.Employee.Plank")]
    public class Plank : MonoBehaviour
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

        public void Initialize()
        {
            controller = GetComponentInParent<Controller>(true);
        }

        private void Update()
        {
            nameLabel.text = EmployeeConfig.Name;
            hireCostLabel.text = EmployeeConfig.HireCost.ToString();
            professionLabel.text = EmployeeConfig.Profession;
        }

        public void SetEmployeeConfig(EmployeeConfig employeeConfig)
        {
            this.employeeConfig = employeeConfig;
        }

        // Called be pressing button.
        public void TryHireEmployee()
        {
            if (controller.TryBuyEmployee(EmployeeConfig).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
