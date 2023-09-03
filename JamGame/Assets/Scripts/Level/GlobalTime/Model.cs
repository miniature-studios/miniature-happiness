using Common;
using System;
using UnityEngine;

namespace Level.GlobalTime
{
    [Serializable]
    public struct Days
    {
        public float Days_;

        public readonly float RealTimeSeconds => Days_ * Model.DayLength;

        public static Days FromRealTimeSeconds(float seconds)
        {
            return new Days() { Days_ = seconds / Model.DayLength };
        }

        public static Days operator +(Days a, Days b)
        {
            return new Days() { Days_ = a.Days_ + b.Days_ };
        }

        public static Days operator -(Days a, Days b)
        {
            return new Days() { Days_ = a.Days_ - b.Days_ };
        }

        public static bool operator >(Days a, Days b)
        {
            return a.Days_ > b.Days_;
        }

        public static bool operator <(Days a, Days b)
        {
            return a.Days_ < b.Days_;
        }

        public static bool operator <=(Days a, Days b)
        {
            return a.Days_ <= b.Days_;
        }

        public static bool operator >=(Days a, Days b)
        {
            return a.Days_ >= b.Days_;
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

        private object setTimeScaleLockHolder = null;

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

        // Called by buttons in UI.
        public void SetTimeScale(float scale)
        {
            if (setTimeScaleLockHolder != null)
            {
                Debug.LogError($"Cannot set timescale: locked by {setTimeScaleLockHolder}");
                return;
            }

            this.scale = scale;
            Time.timeScale = scale;
        }

        public Result SetTimeScaleLock(object sender, float timeScaleOverride)
        {
            if (sender == null)
            {
                return new FailResult("Invalid sender: null");
            }
            else if (setTimeScaleLockHolder != null && sender != setTimeScaleLockHolder)
            {
                return new FailResult($"Cannot set lock: locked by {setTimeScaleLockHolder}");
            }

            setTimeScaleLockHolder = sender;
            Time.timeScale = timeScaleOverride;

            return new SuccessResult();
        }

        public Result RemoveTimeScaleLock(object sender)
        {
            if (sender == null)
            {
                return new FailResult("Invalid sender: null");
            }
            else if (setTimeScaleLockHolder != null && sender != setTimeScaleLockHolder)
            {
                return new FailResult($"Cannot remove lock: locked by {setTimeScaleLockHolder}");
            }

            setTimeScaleLockHolder = null;
            Time.timeScale = scale;

            return new SuccessResult();
        }
    }
}
