using System;
using UnityEngine;

namespace Level.GlobalTime
{
    [Serializable]
    public struct Days
    {
        public float Days_;

        public readonly float RealTimeSeconds => Days_ * Model.DayLength;

        public static Days operator +(Days value1, Days value2)
        {
            return new Days { Days_ = value1.Days_ + value2.Days_ };
        }
    }

    [AddComponentMenu("Scripts/Level.GlobalTime.Model")]
    public class Model : MonoBehaviour
    {
        private static float dayLength_ = 0.0f;
        public static float DayLength => dayLength_;

        [SerializeField]
        private float dayLength;

        private float scale = 1.0f;

        private void Awake()
        {
            if (dayLength_ != 0.0f)
            {
                Debug.LogError(
                    "Two or more instances of Level.GlobalTime.Model are detected in scene! Deleting one instance..."
                );
                Destroy(this);
                return;
            }

            dayLength_ = dayLength;
            Time.timeScale = scale;
        }

        // Called by buttons that changes time scale.
        public void SetTimeScale(float scale)
        {
            this.scale = scale;
            Time.timeScale = scale;
        }
    }
}
