using System;
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
}
