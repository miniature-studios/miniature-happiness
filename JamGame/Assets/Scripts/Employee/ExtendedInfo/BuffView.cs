using Common;
using Pickle;
using UnityEngine;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.BuffView")]
    internal class BuffView : MonoBehaviour, IUidHandle
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public Buff BuffModelPrefab;

        public InternalUid Uid => BuffModelPrefab.Uid;
    }
}
