using System;
using UnityEngine;

namespace Level.GlobalTime
{
    [Serializable]
    public struct Days
    {
        public float Days_;

        public readonly float RealTimeSeconds => Days_ * Model.DayLength;
    }

    [AddComponentMenu("Level.GlobalTime.Model")]
    public class Model : MonoBehaviour
    {
        private static float dayLength_ = 0.0f;
        public static float DayLength => dayLength_;

        [SerializeField] private float dayLength;

        private float scale = 1.0f;

        private void Start()
        {
            if (dayLength_ != 0.0f)
            {
                Debug.LogError("Two or more instances of Level.GlobalTime.Model are detected in scene! Deleting current instance...");
                Destroy(this);
            }

            dayLength_ = dayLength;
            Time.timeScale = scale;
        }

        public void SetTimeScale(float scale)
        {
            this.scale = scale;
            Time.timeScale = scale;
        }
    }
}