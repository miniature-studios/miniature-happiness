using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Location.EmployeeManager
{
    [AddComponentMenu("Scripts/Location/EmployeeManager/Location.EmployeeManager.View")]
    internal class View : MonoBehaviour
    {
        private const string FIRING_ENABLED_LABEL = "finish firing";
        private const string FIRING_DISABLED_LABEL = "start firing";

        [Required]
        [SerializeField]
        private TMP_Text firingButtonLabel;

        [Required]
        [SerializeField]
        private Controller controller;

        private bool displayedFiringMode = false;

        private void Start()
        {
            displayedFiringMode = false;
            firingButtonLabel.text = FIRING_DISABLED_LABEL;
        }

        private void Update()
        {
            if (displayedFiringMode != controller.FiringMode)
            {
                displayedFiringMode = controller.FiringMode;
                firingButtonLabel.text = displayedFiringMode
                    ? FIRING_ENABLED_LABEL
                    : FIRING_DISABLED_LABEL;
            }
        }
    }
}
