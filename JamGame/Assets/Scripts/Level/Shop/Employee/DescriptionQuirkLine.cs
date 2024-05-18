using Employee.Personality;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop/Employee/Level.Shop.Employee.DescriptionQuirkLine")]
    internal class DescriptionQuirkLine : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Image icon;

        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        public void FillData(QuirkConfig quirkConfig)
        {
            icon.sprite = quirkConfig.Icon;
            nameLabel.text = quirkConfig.FullName;
        }
    }
}
