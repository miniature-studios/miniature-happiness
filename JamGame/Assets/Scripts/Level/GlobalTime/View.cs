using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Level.GlobalTime
{
    [AddComponentMenu("Scripts/Level/GlobalTime/Level.GlobalTime.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private ToggleGroup toggleGroup;

        [SerializeField]
        [Required]
        private Model model;

        private Toggle active = null;

        private void Update()
        {
            if (model.IsLocked)
            {
                return;
            }

            Toggle new_active = toggleGroup.GetFirstActiveToggle();
            if (new_active != active)
            {
                active = new_active;
                float value = active.GetComponent<TimeScaleValue>().Value;
                model.SetTimeScale(value);
            }
        }
    }
}
