using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level.Boss.StressView")]
    public class StressView : MonoBehaviour
    {
        [SerializeField] private RectTransform bar;
        [SerializeField] private Model model;

        private void Update()
        {
            bar.localScale = new Vector3(1.0f, Mathf.Clamp01(model.Stress));
        }
    }
}
