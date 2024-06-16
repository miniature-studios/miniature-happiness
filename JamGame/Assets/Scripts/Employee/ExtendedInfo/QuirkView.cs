using Employee.Personality;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.QuirkView")]
    internal class QuirkView : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text label;

        [Required]
        [SerializeField]
        private Image icon;

        public void InitQuirkGraphic(Quirk quirk)
        {
            icon.sprite = quirk.Icon;
            label.text = quirk.FullName;
        }
    }
}
