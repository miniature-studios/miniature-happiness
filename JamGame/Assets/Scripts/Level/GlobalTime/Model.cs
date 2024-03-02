using System;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.GlobalTime
{
    [Serializable]
    [InlineProperty]
    public struct Days
    {
        [SerializeField]
        private float days;
        public readonly float Value => days;

        public readonly RealTimeSeconds RealTimeSeconds => new(days * Model.DayLength);

        public static Days Zero => new(0);

        public Days(RealTimeSeconds seconds)
        {
            days = seconds.Value / Model.DayLength;
        }

        public Days(float days)
        {
            this.days = days;
        }

        public static Days operator +(Days a, Days b)
        {
            return new Days() { days = a.days + b.days };
        }

        public static Days operator -(Days a, Days b)
        {
            return new Days() { days = a.days - b.days };
        }

        public static Days operator *(Days a, float b)
        {
            return new Days() { days = a.days * b };
        }

        public static Days operator *(float a, Days b)
        {
            return new Days() { days = a * b.days };
        }

        public static bool operator >(Days a, Days b)
        {
            return a.days > b.days;
        }

        public static bool operator <(Days a, Days b)
        {
            return a.days < b.days;
        }

        public static bool operator <=(Days a, Days b)
        {
            return a.days <= b.days;
        }

        public static bool operator >=(Days a, Days b)
        {
            return a.days >= b.days;
        }
    }

    [Serializable]
    [InlineProperty]
    public struct RealTimeSeconds
    {
        [SerializeField]
        private float seconds;
        public readonly float Value => seconds;

        public static RealTimeSeconds Zero => new(0);

        public RealTimeSeconds(float seconds)
        {
            this.seconds = seconds;
        }

        public static RealTimeSeconds operator +(RealTimeSeconds a, RealTimeSeconds b)
        {
            return new RealTimeSeconds() { seconds = a.seconds + b.seconds };
        }

        public static RealTimeSeconds operator -(RealTimeSeconds a, RealTimeSeconds b)
        {
            return new RealTimeSeconds() { seconds = a.seconds - b.seconds };
        }

        public static RealTimeSeconds operator *(RealTimeSeconds a, float b)
        {
            return new RealTimeSeconds() { seconds = a.seconds * b };
        }

        public static RealTimeSeconds operator *(float a, RealTimeSeconds b)
        {
            return new RealTimeSeconds() { seconds = a * b.seconds };
        }

        public static bool operator >(RealTimeSeconds a, RealTimeSeconds b)
        {
            return a.seconds > b.seconds;
        }

        public static bool operator <(RealTimeSeconds a, RealTimeSeconds b)
        {
            return a.seconds < b.seconds;
        }

        public static bool operator <=(RealTimeSeconds a, RealTimeSeconds b)
        {
            return a.seconds <= b.seconds;
        }

        public static bool operator >=(RealTimeSeconds a, RealTimeSeconds b)
        {
            return a.seconds >= b.seconds;
        }
    }

    [AddComponentMenu("Scripts/Level/GlobalTime/Level.GlobalTime.Model")]
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

        // Called by buttons that changes time scale.
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
