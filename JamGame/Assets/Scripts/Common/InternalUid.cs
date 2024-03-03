using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common
{
    [Serializable]
    public class InternalUid
    {
        [SerializeField]
        [ReadOnly]
        private int uid = 0;

#if UNITY_EDITOR
        public void GenerateIfEmpty()
        {
            if (uid != 0)
            {
                return;
            }
            uid = UnityEngine.Random.Range(1, int.MaxValue);
        }
#endif

        public override string ToString()
        {
            return uid.ToString();
        }

        public static bool operator ==(InternalUid lhs, InternalUid rhs)
        {
            return lhs.uid == rhs.uid;
        }

        public static bool operator !=(InternalUid lhs, InternalUid rhs)
        {
            return lhs.uid != rhs.uid;
        }

        public override int GetHashCode()
        {
            return uid;
        }

        public bool Equals(InternalUid other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return uid.Equals(other.uid);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InternalUid);
        }
    }
}
