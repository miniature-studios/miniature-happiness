using UnityEngine;

namespace Level.GlobalTime
{
    [AddComponentMenu("Scripts/Level/GlobalTime/Level.GlobalTime.TimeScaleValue")]
    internal class TimeScaleValue : MonoBehaviour
    {
        [SerializeField]
        private float value;

        public float Value => value;
    }
}
