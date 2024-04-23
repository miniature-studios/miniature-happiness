using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop/Employee/Level.Shop.Employee.Card")]
    internal class Card : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        [Required]
        [SerializeField]
        private TMP_Text hireCostLabel;

        [Required]
        [SerializeField]
        private TMP_Text professionLabel;

        public void UpdateData(Plank plank)
        {
            nameLabel.text = plank.EmployeeConfig.Name;
            hireCostLabel.text = plank.EmployeeConfig.HireCost.ToString();
            professionLabel.text = plank.EmployeeConfig.Profession;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
