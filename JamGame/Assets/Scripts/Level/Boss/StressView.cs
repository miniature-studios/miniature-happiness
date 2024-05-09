using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level/Boss/Level.Boss.StressView")]
    public class StressView : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private Slider slider;

        [SerializeField]
        [Required]
        private TMP_Text value;

        [SerializeField]
        [RequiredIn(PrefabKind.InstanceInPrefab | PrefabKind.InstanceInScene)]
        private Model model;

        private void Update()
        {
            float clamped_value = Mathf.Clamp01(model.StressNormalized);
            slider.value = clamped_value;
            value.text = Mathf.RoundToInt(clamped_value * 100.0f).ToString();
        }
    }
}
