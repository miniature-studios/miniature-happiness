﻿using System;
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

        public readonly RealTimeSeconds RealTimeSeconds => new(days * Model.DayLength.Value);

        public static Days Zero => new(0);

        public Days(RealTimeSeconds seconds)
        {
            days = seconds.Value / Model.DayLength.Value;
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

        public static RealTimeSeconds FromDeltaTime()
        {
            return new RealTimeSeconds(Time.deltaTime);
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

        public static bool operator ==(RealTimeSeconds a, RealTimeSeconds b)
        {
            return a.seconds == b.seconds;
        }

        public static bool operator !=(RealTimeSeconds a, RealTimeSeconds b)
        {
            return a.seconds != b.seconds;
        }

        public override bool Equals(object obj)
        {
            if (obj is RealTimeSeconds rts)
            {
                return obj != null && seconds == rts.seconds;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return seconds.GetHashCode();
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

    [Serializable]
    public struct InGameTime
    {
        public enum Unit
        {
            Days,
            Hours,
        };

        [SerializeField]
        private Unit timeUnit;

        [SerializeField]
        private float value;

        public static InGameTime Zero = new() { value = 0, timeUnit = Unit.Days };

        public readonly RealTimeSeconds RealTimeSeconds =>
            timeUnit switch
            {
                Unit.Hours
                    => new RealTimeSeconds(value * Model.DayLength.Value / Model.HOURS_IN_DAY),
                Unit.Days => new RealTimeSeconds(value * Model.DayLength.Value),
                _ => throw new NotImplementedException()
            };

        private float ToDays()
        {
            return timeUnit switch
            {
                Unit.Hours => value / Model.HOURS_IN_DAY,
                Unit.Days => value,
                _ => throw new NotImplementedException()
            };
        }

        public static InGameTime operator +(InGameTime a, InGameTime b)
        {
            return new InGameTime { value = a.ToDays() + b.ToDays(), timeUnit = Unit.Days };
        }

        public static InGameTime operator -(InGameTime a, InGameTime b)
        {
            return new InGameTime { value = a.ToDays() - b.ToDays(), timeUnit = Unit.Days };
        }

        public static InGameTime operator *(InGameTime a, float b)
        {
            a.value *= b;
            return a;
        }

        public static InGameTime operator *(float a, InGameTime b)
        {
            return b * a;
        }

        public static bool operator >(InGameTime a, InGameTime b)
        {
            return a.ToDays() > b.ToDays();
        }

        public static bool operator <(InGameTime a, InGameTime b)
        {
            return a.ToDays() < b.ToDays();
        }

        public static bool operator <=(InGameTime a, InGameTime b)
        {
            return a.ToDays() <= b.ToDays();
        }

        public static bool operator >=(InGameTime a, InGameTime b)
        {
            return a.ToDays() >= b.ToDays();
        }
    }
}
