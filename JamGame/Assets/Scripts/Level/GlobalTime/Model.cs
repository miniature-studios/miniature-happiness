using Common;
using System;
using UnityEngine;

namespace Level.GlobalTime
{
    public struct DaysLived
    {
        public Days Value;
    }

    [Serializable]
    public struct Days
    {
        public float Days_;

        public readonly float RealTimeSeconds => Days_ * Model.DayLength;
    }

    [AddComponentMenu("Scripts/Level.GlobalTime.Model")]
    public class Model : MonoBehaviour, IDataProvider<DaysLived>
    {
        private static float dayLength_ = 0.0f;
        public static float DayLength => dayLength_;

        [SerializeField]
        private float dayLength;

        private float scale = 1.0f;

        [SerializeField]
        [InspectorReadOnly]
        private Days daysLived = new();

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

        // Called by executor when time has passed
        public void DaysGone(Days days)
        {
            daysLived.Days_ += days.Days_;
        }

        public DaysLived GetData()
        {
            return new DaysLived() { Value = daysLived };
        }
    }
}
