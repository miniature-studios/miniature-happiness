using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.BuffView")]
    internal class BuffView : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text buffLabel;

        public void InitBuffGraphic(Buff buff)
        {
            buffLabel.text = buff.FullName;
        }
    }
}
