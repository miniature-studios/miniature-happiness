using System;
using UnityEngine;

namespace DA_Assets.Shared
{
    [Serializable]
    public struct TProp<T> : IEnableable
    {
        [SerializeField] bool _enabled;
        [SerializeField] T _value;

        public TProp(bool enabled, T v) : this()
        {
            _enabled = enabled;
            _value = v;
        }

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public T Value { get => _value; set => this._value = value; }
    }

    [Serializable]
    public struct TProp<T1, T2> : IEnableable
    {
        [SerializeField] bool _enabled;
        [SerializeField] T1 _value1;
        [SerializeField] T2 _value2;

        public TProp(bool enabled, T1 v1, T2 v2) : this()
        {
            _enabled = enabled;
            _value1 = v1;
            _value2 = v2;
        }

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public T1 Value1 { get => _value1; set => _value1 = value; }
        public T2 Value2 { get => _value2; set => _value2 = value; }
    }

    [Serializable]
    public struct TProp<T1, T2, T3> : IEnableable
    {
        [SerializeField] bool _enabled;
        [SerializeField] T1 _value1;
        [SerializeField] T2 _value2;
        [SerializeField] T3 _value3;

        public TProp(bool enabled, T1 v1, T2 v2, T3 v3) : this()
        {
            _enabled = enabled;
            _value1 = v1;
            _value2 = v2;
            _value3 = v3;
        }

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public T1 Value1 { get => _value1; set => _value1 = value; }
        public T2 Value2 { get => _value2; set => _value2 = value; }
        public T3 Value3 { get => _value3; set => _value3 = value; }
    }

    public interface IEnableable
    {
        bool Enabled { get; set; }
    }
}
