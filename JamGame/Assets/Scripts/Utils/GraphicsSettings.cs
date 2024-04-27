using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Utils
{
    [AddComponentMenu("Scripts/Utils/Utils.GraphicsSettings")]
    internal class GraphicsSettings : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;

        [SerializeField]
        private TMP_Dropdown fullscreenMode;

        [SerializeField]
        private Canvas canvas;

        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new InputActions();
            inputActions.Enable();
            inputActions.Debug.OpenGraphicsSettings.performed += ToggleVisibility;
        }

        private void ToggleVisibility(CallbackContext ctx)
        {
            canvas.enabled ^= true;
        }

        // Called by `apply` button.
        public void Apply()
        {
            string resolution_str = resolutionDropdown.options[resolutionDropdown.value].text;
            string[] dimensions = resolution_str.Split('x');
            if (!int.TryParse(dimensions[0], out int width))
            {
                Debug.LogError("Incorrect resolution format in dropdown: " + resolution_str);
            }

            if (!int.TryParse(dimensions[1], out int height))
            {
                Debug.LogError("Incorrect resolution format in dropdown: " + resolution_str);
            }

            string fullscreen_mode_str = fullscreenMode.options[fullscreenMode.value].text;
            FullScreenMode fullscreen_mode = fullscreen_mode_str switch
            {
                "ExclusiveFullScreen" => FullScreenMode.ExclusiveFullScreen,
                "MaximizedWindow" => FullScreenMode.MaximizedWindow,
                "FullScreenWindow" => FullScreenMode.FullScreenWindow,
                "Windowed" => FullScreenMode.Windowed,
                _ => throw new System.Exception()
            };

            Screen.SetResolution(width, height, fullscreen_mode);
        }
    }
}
