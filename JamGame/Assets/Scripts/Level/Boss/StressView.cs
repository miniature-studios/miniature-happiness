using UnityEngine;

namespace Level.Boss
{
    [AddComponentMenu("Scripts/Level.Boss.StressView")]
    public class StressView : MonoBehaviour
    {
        [SerializeField] private RectTransform bar;

        public void OnStressChanged(float stress)
        {
            bar.localScale = new Vector3(1.0f, Mathf.Clamp01(stress));
        }
    }
}