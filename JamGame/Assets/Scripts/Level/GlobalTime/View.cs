using System.Collections.Generic;
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

        private Dictionary<Toggle, TimeScaleValue> cache = new();

        private void Update()
        {
            Toggle new_active = toggleGroup.GetFirstActiveToggle();

            if (new_active != active)
            {
                float value = FetchTimeScaleValue(new_active).Value;
                // If time scale is locked right now retry attempt every frame
                // until it will be unlocked.
                if (model.TrySetTimeScale(value))
                {
                    active = new_active;
                }
            }
        }

        private TimeScaleValue FetchTimeScaleValue(Toggle toggle)
        {
            if (cache.TryGetValue(toggle, out TimeScaleValue value))
            {
                return value;
            }

            TimeScaleValue value_comp = toggle.GetComponent<TimeScaleValue>();
            cache.Add(toggle, value_comp);
            return value_comp;
        }
    }
}
