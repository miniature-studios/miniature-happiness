using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Location.EmployeeManager
{
    [AddComponentMenu("Scripts/Location/EmployeeManager/Location.EmployeeManager.View")]
    internal class View : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Image firingButtonBackground;

        [Required]
        [SerializeField]
        private Button closeButton;

        [Required]
        [SerializeField]
        private Button firingActivationButton;

        [Required]
        [SerializeField]
        private Controller controller;

        [SerializeField]
        private Color disabledColor;

        [SerializeField]
        private Color enabledColor;

        private void Start()
        {
            controller.OnFiringModeChanged += OnFiringModeChanged;
        }

        private void OnFiringModeChanged(bool value)
        {
            firingButtonBackground.color = value ? enabledColor : disabledColor;
            closeButton.gameObject.SetActive(value);
            firingActivationButton.interactable = !value;
        }
    }
}
