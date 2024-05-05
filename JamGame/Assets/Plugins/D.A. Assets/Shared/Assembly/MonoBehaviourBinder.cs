using System;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class MonoBehaviourBinder<T3> where T3 : MonoBehaviour
    {
        protected T3 monoBeh;
        public T3 MonoBeh { get => monoBeh; set => monoBeh = value; }

        public void SetValue<T>(ref T currentValue, T newValue)
        {
            if (typeof(T) == typeof(string))
            {
                if (currentValue == null)
                    currentValue = (T)Convert.ChangeType(string.Empty, typeof(T));
            }
            else if (currentValue == null)
            {
                currentValue = (T)Activator.CreateInstance(typeof(T));
            }

            if (currentValue.Equals(newValue) == false)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(monoBeh);
#endif
            }

            currentValue = newValue;
        }
    }

    public static class MonoBehaviourBinderExtensions
    {
        public static T SetMonoBehaviour<T, T3>(this T type, T3 monoBeh) where T3 : MonoBehaviour where T : MonoBehaviourBinder<T3>
        {
            if (type == null)
            {
                type = (T)Activator.CreateInstance(typeof(T));
            }

            if (type.MonoBeh == null)
                type.MonoBeh = monoBeh;

            return type;
        }
    }
}