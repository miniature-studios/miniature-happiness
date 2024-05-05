using System;
using UnityEngine;

namespace DA_Assets.Shared
{
    [Serializable]
    public class UpdateBool
    {
        [SerializeField] bool _value;
        [SerializeField] bool _temp;

        public UpdateBool(bool value, bool temp)
        {
            _value = value;
            _temp = temp;
        }

        public bool Value { get => _value; set => _value = value; }
        public bool Temp { get => _temp; set => _temp = value; }
    }
}